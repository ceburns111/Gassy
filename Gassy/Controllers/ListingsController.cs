using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;
using Gassy.Authorization;
using Gassy.Entities; 

namespace Gassy.Controllers
{
    [ApiController]
    [Route("listings")]
    public class ReverbController : ControllerBase
    {
        private IListingService _reverbService;    

        public ReverbController(IListingService reverbService)
        {
            _reverbService = reverbService;
        }

        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetListings()
        {
            var listings = await _reverbService.GetListings();
            return Ok(listings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Listing>> GetListing(int id)
        {
            var listing = await _reverbService.GetListing(id);
            return Ok(listing);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPut("update")]
        public async Task<ActionResult<IEnumerable<Listing>>> UpdateListing(Listing listing)
        {
            var updatedListing = await _reverbService.UpdateListing(listing);
            return Ok(updatedListing);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPost("new")]
        public async Task<ActionResult<Listing>> CreateListing(Listing reverbListing)
        {
            var listing = await _reverbService.CreateListing(reverbListing);
            return Ok(listing);
        }

        [Authorize(RoleId.Admin, RoleId.Agent)]
        [HttpPost("delete/{id}")]
        public async Task<ActionResult<int>> DeleteListing(int id)
        {
            var listingId = await _reverbService.DeleteListing(id);
            return Ok($"Listing Deleted:{listingId}");
        }
    }
}