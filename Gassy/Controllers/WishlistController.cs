using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Gassy.Entities;
using Microsoft.AspNetCore.Mvc;
using Gassy.Authorization;

namespace Gassy.Controllers
{
    [ApiController]
    [Route("wishlist")]
    public class WishlistController : ControllerBase
    {
        private IWishlistService _wishlistService;    

        public WishlistController(IWishlistService reverbService)
        {
            _wishlistService = reverbService;
        }

        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<WishlistItem>>> GetWishlistItems()
        {
            var wishlist = await _wishlistService.GetWishlistItems();
            return Ok(wishlist);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WishlistItem>> GetWishlistItem(int id)
        {
            var listing = await _wishlistService.GetWishlistItem(id);
            return Ok(listing);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPut("update")]
        public async Task<ActionResult<IEnumerable<WishlistItem>>> UpdateWishlistItem(WishlistItem listing)
        {
            var updatedWishlistItem = await _wishlistService.UpdateWishlistItem(listing);
            return Ok(updatedWishlistItem);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPost("new")]
        public async Task<ActionResult<WishlistItem>> CreateWishlistItem(WishlistItem reverbWishlistItem)
        {
            var listing = await _wishlistService.CreateWishlistItem(reverbWishlistItem);
            return Ok(listing);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPost("delete/{id}")]
        public async Task<ActionResult<int>> DeleteWishlistItem(int id)
        {
            var listingId = await _wishlistService.DeleteWishlistItem(id);
            return Ok($"WishlistItem Deleted:{listingId}");
        }
    }
}