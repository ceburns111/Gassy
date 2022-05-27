
using System.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using MySql.Data.MySqlClient;

using Gassy.Models.ReverbModels;

namespace Gassy.Services
{
    public class ReverbService //: IListingService 
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ReverbService(IConfiguration configuration)
        {
            _configuration = configuration;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = "";
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

           public async Task<IEnumerable<ReverbListing>> GetAllListings(){
            throw new NotImplementedException();
            var query = "";
            using var conn = new MySqlConnection(connString);
            var listings = await conn.QueryAsync<ReverbListing>(query, CommandType.Text, commandTimeout: 0);
            if (listings != null) 
                return listings;
            return null;
        }


         public async Task<ReverbListing> UpdateListing(ReverbListing listing) {
            throw new NotImplementedException();
            var query = "";
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing; 
         }


        public async Task<ReverbListing> AddListing(ReverbListing listing){
            throw new NotImplementedException();
            var query = "";
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing;
        }
    }
}