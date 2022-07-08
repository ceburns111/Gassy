using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gassy.Authorization;
using Gassy.Entities; 
using Gassy.Models.Users; 
using Gassy.Models;
using BCryptNet = BCrypt.Net.BCrypt;

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

        Task RevokeToken(string token, string ipAddress);

        Task<User> GetById(int id);
        Task<IEnumerable<User>> GetAll(); 
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        private IJwtUtils _jwtUtils;

        private AppSettings _appSettings;

        private readonly string connString;

        public UserService(IConfiguration configuration, IJwtUtils jwtUtils, AppSettings appSettings)
        {
            _configuration = configuration;
            _jwtUtils = jwtUtils; 
            _appSettings = appSettings;
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
                    Email
                FROM User 
                WHERE UserName = '{model.UserName}' 
                    AND UserPassword = '{model.UserPassword}'
            ";
    
            var connection = new MySqlConnection(connString);
            var user = (await connection.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault();
            Console.WriteLine($"username:{user.UserName}, role: {user.RoleId}");
            if (user == null)
                throw new AppException("Username or password is incorrect");
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);
            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);

        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}"); 
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            await RemoveOldRefreshTokens(user);

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task RevokeToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // revoke token and save
            var updatedToken = RevokeRefreshToken(refreshToken.Token, ipAddress, "Revoked without replacement");
            //Add dapper query to update RefreshToken table 
        }
      

        private Task<User> GetUserByRefreshToken(string token)
        {
            throw new NotImplementedException();
        }

        private Task<RefreshToken> RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            throw new NotImplementedException(); 
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

        public async Task<IEnumerable<User>> GetAll() 
        {
            var query = $"SELECT * FROM User";
            using var conn = new MySqlConnection(connString);
            var users = await conn.QueryAsync<User>(query, CommandType.Text, commandTimeout: 0);
            if (users == null)
                throw new KeyNotFoundException("Users not found");
            return users; 
        }

        private async Task RemoveOldRefreshTokens(User user)
        { 
            var query = $@" 
                DELETE FROM 
                    RefreshToken 
                WHERE UserId = {user.Id}
                    AND Revoked != NULL
                    AND '{_appSettings.RefreshTokenTTL:yyyy-MM-dd hh:mm:ss}'  <= '{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss}'";
                var connection = new MySqlConnection(connString);
                await connection.ExecuteAsync(query);
        }
  

        private async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
        {
            if(!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childTokens = await GetTokenChildren(refreshToken.Token);
                foreach (var child in childTokens) {
                    await RevokeRefreshToken(child.Token, ipAddress, reason);
                }
            }
        }

        //Recursively get children
        private async Task<IEnumerable<RefreshToken>> GetTokenChildren(string parentToken) {
            var query = $@"USE gassydb;
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
            var query = $@"USE gassydb;
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
    }
}