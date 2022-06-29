using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Gassy.Entities;
using Microsoft.AspNetCore.Mvc;
using Gassy.Authorization;

namespace Gassy.Controllers
{
    [Authorize]
    [ApiController]
    [Route("wishlist")]
    public class WishlistController : ControllerBase
    {
        private IWishlistService _wishlistService;    

        public WishlistController(IWishlistService reverbService)
        {
            _wishlistService = reverbService;
        }

        
        [HttpGet("items/{userId}")]
        public async Task<ActionResult<IEnumerable<WishlistItem>>> GetWishlistItems(int userId)
        {
            var wishlist = await _wishlistService.GetWishlist(userId);
            return Ok(wishlist);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WishlistItem>> GetWishlistItem(int id)
        {
            var listing = await _wishlistService.GetWishlistItem(id);
            return Ok(listing);
        }

        [HttpPut("update")]
        public async Task<ActionResult<IEnumerable<WishlistItem>>> UpdateWishlistItem(WishlistItem listing)
        {
            var updatedWishlistItem = await _wishlistService.UpdateWishlistItem(listing);
            return Ok(updatedWishlistItem);
        }

        [HttpPost("new")]
        public async Task<ActionResult<WishlistItem>> CreateWishlistItem(WishlistItem reverbWishlistItem)
        {
            var listing = await _wishlistService.CreateWishlistItem(reverbWishlistItem);
            return Ok(listing);
        }

        [HttpPost("delete/{id}")]
        public async Task<ActionResult<int>> DeleteWishlistItem(int id)
        {
            var listingId = await _wishlistService.DeleteWishlistItem(id);
            return Ok($"WishlistItem Deleted:{listingId}");
        }
    }
}