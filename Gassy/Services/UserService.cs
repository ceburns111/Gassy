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
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<User> GetById(int id);
        Task<IEnumerable<User>> GetAll(); 
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        private IJwtUtils _jwtUtils;

        private readonly string connString;

        public UserService(IConfiguration configuration, IJwtUtils jwtUtils)
        {
            _configuration = configuration;
            _jwtUtils = jwtUtils; 
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
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
            return new AuthenticateResponse(user,  _jwtUtils.GenerateJwtToken(user));
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

        // public string GenerateJwtToken(User user)
        // {
        //     // generate token that is valid for 7 days
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
        //         Expires = DateTime.UtcNow.AddDays(7),
        //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //     };
        //     var token = tokenHandler.CreateToken(tokenDescriptor);
        //     return tokenHandler.WriteToken(token);
        // }
    }
}