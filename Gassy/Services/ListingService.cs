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
    public interface IListingService
    {
        
        Task<IEnumerable<Listing>> GetListings();

        Task<Listing> AddListing(Listing listing); 
    }

    public class ListingService : IListingService {
        private readonly AppSettings _appSettings;

        //private readonly ListingContext _context;
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ListingService(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSettings = appSettings.Value; 
            _configuration = configuration;
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
    }

  
}