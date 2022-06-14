using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("ReverbListings")]
    public class ReverbController : ControllerBase
    {
        private IReverbListingService _reverbService;    

        public ReverbController(IReverbListingService reverbService)
        {
            _reverbService = reverbService;
        }

        
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetListings()
        {
            var listings = await _reverbService.GetListings();
            return Ok(listings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ListingDto>> GetListing(int id)
        {
            var listing = await _reverbService.GetListing(id);
            return Ok(listing);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<ActionResult<IEnumerable<ListingDto>>> UpdateListing(ListingDto listing)
        {
            var updatedListing = await _reverbService.UpdateListing(listing);
            return Ok(updatedListing);
        }

        [Authorize]
        [HttpPost("New")]
        public async Task<ActionResult<ListingDto>> CreateListing(ListingDto reverbListing)
        {
            var listing = await _reverbService.CreateListing(reverbListing);
            return Ok(listing);
        }

        [Authorize]
        [HttpPost("Delete/{id}")]
        public async Task<ActionResult<int>> DeleteListing(int id)
        {
            var listingId = await _reverbService.DeleteListing(id);
            return Ok($"Listing Deleted:{listingId}");
        }
    }
}