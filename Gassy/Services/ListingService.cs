using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gassy.Models;
using Gassy.Authorization;
using Gassy.Models.Users;
using Gassy.Helpers;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Linq;
using MySql.Data.MySqlClient;
using Gassy.Entities; 

namespace Gassy.Services
{
   public interface IListingService
    {
        Task<IEnumerable<Listing>> GetListings(); 
        Task<Listing> GetListing(int id);
        Task<Listing> CreateListing(Listing listing); 
        Task<Listing> UpdateListing(Listing listing);
        Task<int> DeleteListing(int reverbId);
    }

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

        public async Task<IEnumerable<Listing>> GetListings(){
            var query = $@"
                SELECT *
                FROM Listing"; 
            using var conn = new MySqlConnection(connString);
            var listings = await conn.QueryAsync<Listing>(query, CommandType.Text, commandTimeout: 0);
            if (listings != null) 
                return listings;
            return null; 
        }


        public async Task<Listing> GetListing(int id) {
            var query = $@"
                SELECT *
                FROM Listing
                WHERE Id = {id}"; 
            using var conn = new MySqlConnection(connString);
            var listing = (await conn.QueryAsync<Listing>(query, CommandType.Text, commandTimeout: 0)).FirstOrDefault(); 
            if (listing != null) 
                return listing;
            return null; 
        }

         public async Task<Listing> UpdateListing(Listing listing) {
            var query = $@"
                UPDATE Listing
                SET  
                    Make = '{listing.Make}', 
                    Model = '{listing.Model}',
                    Price = '{listing.Price}',
                    ItemDescription = '{listing.ItemDescription}',
                    ItemCondition = '{listing.ItemCondition}',
                    OffersEnabled = {listing.OffersEnabled},
                    Link = '{listing.Link}',
                    CategoryId = {(int)listing.Category},
                    CreatedAt = '{listing.CreatedAt:yyyy-MM-dd hh:mm:ss}',
                    UpdatedAt = '{listing.UpdatedAt:yyyy-MM-dd hh:mm:ss}'
                WHERE Id = '{listing.Id}'
            "; 
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing; 
         }


        public async Task<Listing> CreateListing(Listing listing){
            var query = $@"
                INSERT INTO 
                    Listing (
                        SiteId, 
                        Make, 
                        Model,
                        Price,
                        ItemDescription,
                        ItemCondition,
                        OffersEnabled,
                        Link,
                        CategoryId,
                        CreatedAt,
                        UpdatedAt
                        )
                    Values(
                        '{listing.SiteId}', 
                        '{listing.Make}', 
                        '{listing.Model}' , 
                        {listing.Price},
                        '{listing.ItemDescription}',
                        '{listing.ItemCondition}',
                        {listing.OffersEnabled},
                        '{listing.Link}',
                        {(int)listing.Category},
                        '{listing.CreatedAt:yyyy-MM-dd hh:mm:ss}',
                        '{listing.UpdatedAt:yyyy-MM-dd hh:mm:ss}'
                        )";


            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return listing;
        }

        public async Task<int> DeleteListing(int id) {
             var query = $@"
                DELETE FROM Listing 
                WHERE Id = {id}";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return id; 
        }
    }
}