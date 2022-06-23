using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Gassy.Entities;
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
using System;

namespace Gassy.Services
{
   public interface IWishlistService
    {
        Task<IEnumerable<WishlistItem>> GetWishlistItems(); 
        Task<WishlistItem> GetWishlistItem(int id);
        Task<WishlistItem> CreateWishlistItem(WishlistItem listing); 
        Task<WishlistItem> UpdateWishlistItem(WishlistItem listing);
        Task<int> DeleteWishlistItem(int reverbId);
    }

    public class WishlistService : IWishlistService { 

        private readonly IConfiguration _configuration;
        private readonly string connString;

        public WishlistService(IConfiguration configuration)
        {
            _configuration = configuration;
            var host =  _configuration["ConnectionStrings:DBHOST"] ?? _configuration.GetConnectionString("DBHOST");
            var port =  _configuration["ConnectionStrings:DBPORT"] ?? _configuration.GetConnectionString("DBPORT");
            var password = _configuration["ConnectionStrings:MYSQL_PASSWORD"] ?? _configuration.GetConnectionString("MYSQL_PASSWORD");
            var userName = _configuration["ConnectionStrings:MYSQL_USER"] ?? _configuration.GetConnectionString("MYSQL_USER");
            var db  = _configuration["ConnectionStrings:MYSQL_DATABASE"] ?? _configuration.GetConnectionString("MYSQL_DATABASE");
            connString = $"Server={host}; Uid={userName}; Pwd={password};Port={port}; Database={db}";
        }

        public async Task<IEnumerable<WishlistItem>> GetWishlistItems(){
            throw new NotImplementedException(); 
        }
        
        public async Task<WishlistItem> GetWishlistItem(int id) {
            throw new NotImplementedException(); 
        }

        public async Task<WishlistItem> CreateWishlistItem(WishlistItem listing) {
              throw new NotImplementedException(); 
        }

        public async  Task<WishlistItem> UpdateWishlistItem(WishlistItem listing) {
              throw new NotImplementedException(); 
        }
        
        public async Task<int> DeleteWishlistItem(int reverbId) {
              throw new NotImplementedException(); 
        }
    }
}    