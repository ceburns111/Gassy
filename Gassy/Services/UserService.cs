using Microsoft.Extensions.Options;
using Gassy.Authorization;
using Gassy.Entities; 
using Gassy.Models.Users; 
using Gassy.Models;
using BCrypt.Net;
using Gassy.Helpers;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using MySql.Data.MySqlClient;

namespace Gassy.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model,  string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        
        Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserId(int userId);
        
        Task RevokeToken(string token, string ipAddress);

        Task DeleteUser(int id);

        Task<User> GetById(int id);
        Task<IEnumerable<User>> GetAll(); 

        Task AddUser(SignupRequest signupRequest);
        Task EditUser(EditUserRequest editRequest);

    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        private readonly IJwtUtils _jwtUtils;

        private readonly AppSettings _appSettings;

        private readonly string connString;

        public UserService(IConfiguration configuration, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _jwtUtils = jwtUtils; 
            _appSettings = appSettings.Value;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            Console.WriteLine($"Attempting to authenticate user: {model.UserName}");
            string query = $@"
                SELECT 
                    Id,
                    FirstName,
                    LastName,
                    RoleId,
                    UserName,
                    Email,
                    PasswordHash
                FROM User 
                WHERE UserName = '{model.UserName}'
            ";

            var connection = new MySqlConnection(connString);
            var user = (await connection.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.UserPassword, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);

            //Remove expired refresh tokens            
            await RemoveExpiredRefreshTokens(user);

            //Add the new token to database 
            await AddNewRefreshToken(refreshToken, user.Id);
           
            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);

        }

        public async Task<AuthenticateResponse> RefreshToken(string tokenStr, string ipAddress)
        {
            Console.WriteLine($"Refreshing Token...");
            Console.WriteLine($"Token: {tokenStr}");
            Console.WriteLine($"Ip Address: {ipAddress}");
            var refreshToken = await GetRefreshTokenByTokenStr(tokenStr);
            Console.WriteLine($"Refresh Token: {refreshToken.Token}");
            var user = await GetUserByRefreshToken(tokenStr);
            Console.WriteLine($"User ID: {user.Id}");

            if (refreshToken.IsRevoked)
            {
                Console.WriteLine("Refresh token is revoked, removing all decedant tokens");
                await RevokeDescendantRefreshTokens(refreshToken, ipAddress, $"Attempted reuse of revoked ancestor token: {tokenStr}"); 
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            Console.WriteLine("Rotating refresh tokens...");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress);
            Console.WriteLine($"New Refresh Token: {newRefreshToken}");
            //remove old refresh tokens from user
            await RemoveExpiredRefreshTokens(user); //this 

            Console.WriteLine("Generating new JWT Token...");
            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task RevokeToken(string tokenStr, string ipAddress)
        {
            var token = await GetRefreshTokenByTokenStr(tokenStr);

            if (!token.IsActive)
                throw new AppException("Invalid token");

            await RevokeRefreshToken(token.Token, ipAddress, "Revoked without replacement");
            //Add dapper query to update RefreshToken table 
        }

        public async Task<User> GetById(int id)
        {
            string query = $"SELECT * FROM User WHERE Id = {id}";
            using var connection = new MySqlConnection(connString);
            var user = (await connection.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault(); 
            if (user == null) 
                throw new KeyNotFoundException("User not found");
            return user; 
        }

        public async Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserId(int userId)
        {
            string query = $"SELECT * FROM RefreshToken WHERE UserId = {userId}";
            using var connection = new MySqlConnection(connString);
            var refreshTokens = await connection.QueryAsync<RefreshToken>(query, CommandType.Text, commandTimeout: 0);
            if (refreshTokens == null) 
                throw new KeyNotFoundException($"No refresh tokens found for {userId}");
            return refreshTokens; 
        }

        public async Task<IEnumerable<User>> GetAll() 
        {
            var query = $"SELECT * FROM User";
            using var conn = new MySqlConnection(connString);
            var users = await conn.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0);
            if (users == null)
                throw new KeyNotFoundException("Users not found");
            return users; 
        }

        private async Task<User> GetUserByRefreshToken(string token)
        {
            var query = $@"SELECT 
                                u.Id,
                                u.FirstName,
                                u.LastName,
                                u.Email,
                                u.UserName,
                                u.RoleId
                            FROM User AS u
                            INNER JOIN RefreshToken AS r
                                ON u.Id = r.UserId
                            WHERE r.Token = '{token}';";

            var connection = new MySqlConnection(connString);
            var user = (await connection.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return user;
        }

        private async Task<RefreshToken> RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            await RevokeRefreshToken(refreshToken.Token, refreshToken.CreatedByIp, "Replaced by new token", newRefreshToken.Token);
            await AddNewRefreshToken(newRefreshToken, refreshToken.UserId);
            return newRefreshToken;
        }

        private async Task RemoveExpiredRefreshTokens(User user)
        { 
            var query = $@" 
                DELETE FROM 
                    RefreshToken 
                WHERE UserId = {user.Id}
                    AND Revoked != NULL
                    AND DATE_ADD(Created, INTERVAL {_appSettings.RefreshTokenTTL} DAY)  <= '{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}'";
                    
            var connection = new MySqlConnection(connString);
            await connection.ExecuteAsync(query);
        }

        private async Task<RefreshToken> GetRefreshTokenByTokenStr(string token)
        { 
            string query = $"SELECT * FROM RefreshToken WHERE Token = '{token}'";
            using var connection = new MySqlConnection(connString);
            var refreshToken = (await connection.QueryAsync<RefreshToken>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault();
            if (refreshToken == null) 
                throw new KeyNotFoundException($"No refresh token found for Refresh Token: {token}");
            return refreshToken;
        }
  
        private async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
        {
            Console.WriteLine("Revoking Descendants");
            if(!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childTokens = await GetTokenChildren(refreshToken.Token);
                foreach (var child in childTokens) {
                    await RevokeRefreshToken(child.Token, ipAddress, reason);
                }
            }
        }

        private async Task AddNewRefreshToken(RefreshToken token, int userId) {
               var query = $@"
                            INSERT INTO RefreshToken(
                                Token, 
                                UserId, 
                                Expires, 
                                Created, 
                                CreatedByIp
                            )
                            VALUES (
                                '{token.Token}',
                                 {userId},
                                '{token.Expires:yyyy-MM-dd hh:mm:ss}',
                                '{token.Created:yyyy-MM-dd hh:mm:ss}',
                                '{token.CreatedByIp}'
                            )";

            var connection = new MySqlConnection(connString);
            await connection.ExecuteAsync(query);
        }

        //Recursively get children
        private async Task<IEnumerable<RefreshToken>> GetTokenChildren(string parentToken) {
            var query = $@"
                            WITH RECURSIVE cte (Id, UserId, Token, ReplacedbyToken) AS (
                            SELECT	Id,
                                        UserId,
                                        Token,
                                        ReplacedByToken
                            FROM      RefreshToken
                            WHERE     Token = '{parentToken}'
                            UNION ALL
                            
                            SELECT 	r.Id,
                                        r.UserId,
                                        r.Token,
                                        r.ReplacedByToken
                            FROM      RefreshToken r
                            INNER JOIN cte
                                ON r.Token = cte.ReplacedByToken
                            )
                            SELECT Id, Token, ReplacedByToken FROM cte";

            var connection = new MySqlConnection(connString);
            var children = await connection.QueryAsync<RefreshToken>(query, CommandType.Text, commandTimeout: 0);
            return children; 
        }

        private async Task RevokeRefreshToken(string token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            var query = $@"
                UPDATE RefreshToken
                SET
                    Revoked = '{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}',
                    RevokedByIp = '{ipAddress}',
                    ReasonRevoked = '{reason}',
                    ReplacedByToken = '{replacedByToken}'
                WHERE 
                    Token = '{token}'
                    ";

            var connection = new MySqlConnection(connString);
            await connection.ExecuteAsync(query);
        }
        
        public async Task AddUser(SignupRequest request) {
            var users = await GetAll();
            var usernames = users.Select(x => x.UserName);

            if (usernames.Contains(request.UserName))
                throw new AppException($"Username {request.UserName} is already taken");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.UserPassword);

            string query = $@"
             INSERT INTO User(FirstName, LastName, PhoneNumber, Email, UserName, PasswordHash, RoleId)
                VALUES ('{request.FirstName}', '{request.LastName}', '{request.PhoneNumber}'
                , '{request.Email}', '{request.UserName}', '{passwordHash}', 1)
            ";
    
            var connection = new MySqlConnection(connString);
            
            await connection.ExecuteAsync(query);

            Console.WriteLine($"Added User: {request.UserName}");          
        }

        public async Task EditUser(EditUserRequest editRequest) {
       
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(editRequest.UserPassword);
    
            string query = $@"
             UPDATE User
             SET
                FirstName = '{editRequest.FirstName}',
                LastName = '{editRequest.LastName}',
                PhoneNumber = '{editRequest.PhoneNumber}',
                Email = '{editRequest.Email}',
                PasswordHash = '{passwordHash}'
            WHERE Id = {editRequest.UserId}
            ";
    
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
        }

        public async Task DeleteUser(int id) {
                    string query = $@"DELETE FROM User WHERE Id = {id}";
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
        }
    }
}