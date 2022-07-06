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
        Task<IEnumerable<WishlistItem>> GetWishlist(int ownerId); 
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

        public async Task<IEnumerable<WishlistItem>> GetWishlist(int ownerId){
             var query = $@"
                SELECT *
                FROM WishlistItem
                where OwnerId = {ownerId}"; 
            using var conn = new MySqlConnection(connString);
            var items = await conn.QueryAsync<WishlistItem>(query, CommandType.Text, commandTimeout: 0);
            if (items != null) 
                return items;
            return null; 
        }
        
        public async Task<WishlistItem> GetWishlistItem(int id) {
            var query = $@"
                SELECT *
                FROM WishlistItem
                WHERE Id = {id}"; 
            using var conn = new MySqlConnection(connString);
            var item = (await conn.QueryAsync<WishlistItem>(query, CommandType.Text, commandTimeout: 0))
                .FirstOrDefault();
            if (item != null) 
                return item ;
            return null;         
        }


        public async Task<WishlistItem> CreateWishlistItem(WishlistItem item) {
               var query = $@"
                INSERT INTO 
                    WishlistItem (
                        Make, 
                        Model,
                        MinPrice,
                        MaxPrice,
                        CreatedAt,
                        CategoryId,
                        OwnerId
                        )
                    Values(
                        '{item.Make}', 
                        '{item.Model}' , 
                         {item.MinPrice},
                         {item.MaxPrice},
                        '{DateTime.Now:yyyy-MM-dd hh:mm:ss}',
                        '{(int)item.Category}',
                        '{item.OwnerId}'
                        )";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return item;
        }

        public async  Task<WishlistItem> UpdateWishlistItem(WishlistItem item) {
              var query = $@"
                UPDATE WishlistItem
                SET  
                        Make = '{item.Make}',
                        Model = '{item.Model}',
                        MinPrice = {item.MinPrice},
                        MaxPrice = {item.MaxPrice},
                        CategoryId = {(int)item.Category},
                        UpdatedAt = '{DateTime.Now:yyyy-MM-dd hh:mm:ss}',
                        OwnerId = {item.OwnerId}
                WHERE Id = '{item.Id}'
            "; 
            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return item; 
        }
        
        public async Task<int> DeleteWishlistItem(int id) {
        var query = $@"
                    DELETE FROM WishlistItem
                    WHERE Id = {id}";

            using var conn = new MySqlConnection(connString);
            await conn.ExecuteAsync(query);
            return id;         
        }
    }
}    