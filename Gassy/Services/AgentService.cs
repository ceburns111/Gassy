using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gassy.Models;
using Gassy.Helpers;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using MySql.Data.MySqlClient;


namespace Gassy.Services
{
      public interface IAgentService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        Task<Agent> GetById(int id);
        Task<Listing> GetListing(int id);
        Task<IEnumerable<Listing>> GetListings();

        Task<Listing> AddListing(Listing listing); 
    }

    public class AgentService : IAgentService
    {
        private readonly AppSettings _appSettings;

        //private readonly ListingContext _context;
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public AgentService(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSettings = appSettings.Value; 
            _configuration = configuration;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
                string query = $"SELECT * FROM agent WHERE Agentname = '{model.AgentName}' and AgentPassword = '{model.AgentPassword}'";
                using (var connection = new MySqlConnection(connString))
                {
                    var agent = connection.Query<Agent>(query, CommandType.Text, commandTimeout: 0).FirstOrDefault();     
                    if (agent == null) return null;
                    return new AuthenticateResponse(agent,  GenerateJwtToken(agent));
                }
        }

        public async Task<Listing> GetListing(int id){
            var query = $"select * from listing where id = {id}"; 
            using (var conn = new MySqlConnection((connString))){
                var listing = await conn.QueryAsync<Listing>(query, CommandType.Text, commandTimeout: 0);
                if (listing != null) return listing.FirstOrDefault(); 
                return null;
            }
        }

          public async Task<IEnumerable<Listing>> GetListings(){
            var query = $@"select * from listing"; 
            using (var conn = new MySqlConnection((connString))){
                var listings = await conn.QueryAsync<Listing>(query, CommandType.Text, commandTimeout: 0);
                if (listings != null) return listings; 
                return null;
            }
        }

        public async Task<Listing> AddListing(Listing listing){
            var query = $@"
                INSERT INTO listing (SiteId, Make, Model, Price)
                Values('{listing.SiteId}', '{listing.Make}', '{listing.Model}', {listing.Price})";
                //, '{listing.CreatedAt}', '{listing.UpdatedAt}');"; 


            using (var conn = new MySqlConnection((connString))){
                await conn.ExecuteAsync(query);
                return listing; 
            }
        }

        public async Task<Agent> GetById(int id)
        {
            //throw new NotImplementedException(); 
            // try {
                string query = $"SELECT * FROM agent where id = {id}";
                using (var connection = new MySqlConnection(connString)) {
                    var agents = await connection.QueryAsync<Agent>(query, CommandType.Text, commandTimeout: 0);
                    var agent = agents.FirstOrDefault(); 
                    return agent; 
                }
        }

        public string GenerateJwtToken(Agent agent)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", agent.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}