using Microsoft.AspNetCore.Mvc;
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
using System.Linq;
using MySql.Data.MySqlClient;
using GassyFunctionHelpers.Models;

namespace Gassy.Services
{
  
    public class ReverbListingService : IReverbListingService { 
        
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ReverbListingService(IConfiguration configuration)
        {
            _configuration = configuration;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public async Task<IEnumerable<ListingDto>> GetListings(){
            var query = $@"
                SELECT *
                FROM ReverbListing"; 
            using var conn = new MySqlConnection(connString);
            var listings = await conn.QueryAsync<ListingDto>(query, CommandType.Text, commandTimeout: 0);
            if (listings != null) 
                return listings;
            return null; 
        }


        public async Task<ListingDto> GetListing(int reverbId) {
            var query = $@"
                SELECT *
                FROM ReverbListing
                WHERE ReverbId = {reverbId}"; 
            using var conn = new MySqlConnection(connString);
            var listing = (await conn.QueryAsync<ListingDto>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault(); 
            if (listing != null) 
                return listing;
            return null; 
        }

         public async Task<ListingDto> UpdateListing(ListingDto listing) {
            var query = $@"
                UPDATE ReverbListing
                SET  
                    Make = '{listing.Make}', 
                    Model = '{listing.Model}',
                    Price = '{listing.Price}',
                    ItemDescription = '{listing.ItemDescription}',
                    ItemCondition = '{listing.ItemCondition}',
                    OffersEnabled = {listing.OffersEnabled},
                    Link = '{listing.Link}',
                    ListingCreatedAt = '{listing.ListingCreatedAt:yyyy-MM-dd HH:mm:ss}',
                    ListingPublishedAt = '{listing.ListingPublishedAt:yyyy-MM-dd HH:mm:ss}',
                    UpdatedAt = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'
                WHERE ReverbId = '{listing.ReverbId}'
            "; 
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing; 
         }


        public async Task<ListingDto> CreateListing(ListingDto listing){
            var query = $@"
                INSERT INTO 
                    ReverbListing (
                        ReverbId, 
                        Make, 
                        Model,
                        Price,
                        ItemDescription,
                        ItemCondition,
                        OffersEnabled,
                        Link,
                        ListingCreatedAt,
                        ListingPublishedAt,
                        UpdatedAt
                        )
                    Values(
                        '{listing.ReverbId}', 
                        '{listing.Make}', 
                        '{listing.Model}' , 
                        {listing.Price},
                        '{listing.ItemDescription}',
                        '{listing.ItemCondition}',
                        {listing.OffersEnabled},
                        '{listing.Link}',
                        '{listing.ListingCreatedAt:yyyy-MM-dd HH:mm:ss}',
                        '{listing.ListingPublishedAt:yyyy-MM-dd HH:mm:ss}',
                        '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'
                        )";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing;
        }

        public async Task<int> DeleteListing(int reverbId) {
             var query = $@"
                DELETE FROM ReverbListing 
                WHERE ReverbId = {reverbId}";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return reverbId; 
        }
    }
}