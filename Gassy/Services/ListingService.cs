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
  
    public class ListingService : IListingService { 
        
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ListingService(IConfiguration configuration)
        {
            _configuration = configuration;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public async Task<IEnumerable<Listing>> GetAllListings(){
            var query = $@"select * from listing"; 
            using var conn = new MySqlConnection(connString);
            var listings = await conn.QueryAsync<Listing>(query, CommandType.Text, commandTimeout: 0);
            if (listings != null) 
                return listings;
            return null;
        }


         public async Task<Listing> UpdateListing(Listing listing) {
            var query = $@"
                UPDATE listing
                SET  
                    Make = '{listing.Make}', 
                    Model = '{listing.Model}',
                    Price = '{listing.Price}',
                    ItemDescription = '{listing.ItemDescription}',
                    ItemCondition = '{listing.ItemCondition}',
                    OffersEnabled = {listing.OffersEnabled},
                    Link = '{listing.Link}',
                    ListingUpdatedAt = '{listing.ListingUpdatedAt}',
                    UpdatedAt = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'
                WHERE SiteId = '{listing.SiteId}'
            "; 
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing; 
         }


        public async Task<Listing> AddListing(Listing listing){
            var query = $@"
                INSERT INTO 
                listing (
                    SiteId, 
                    Make, 
                    Model,
                    Price,
                    ItemDescription,
                    ItemCondition,
                    OffersEnabled,
                    Link,
                    ListingCreatedAt,
                    ListingUpdatedAt,
                    UpdatedAt
                    )
                Values(
                    '{listing.SiteId}', 
                    '{listing.Make}' , 
                    '{listing.Model}' , 
                     {listing.Price},
                    '{listing.ItemDescription}',
                    '{listing.ItemCondition}',
                    {listing.OffersEnabled},
                    '{listing.Link}',
                    '{listing.ListingCreatedAt}',
                    '{listing.ListingUpdatedAt}',
                    '{listing.UpdatedAt}'
                    )";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing;
        }
    }

  
}