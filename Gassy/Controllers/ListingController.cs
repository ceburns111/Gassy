using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("listings")]
    public class ListingController : ControllerBase
    {
        private IListingService _listingService;    

        public ListingController(IListingService listingService)
        {
            _listingService = listingService;
        }

        
        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetListing()
        {
            var listings = await _listingService.GetAllListings();
            return Ok(listings);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<IEnumerable<Listing>>> UpdateListing(Listing listing)
        {
            var updatedListing = await _listingService.UpdateListing(listing);
            return Ok(updatedListing);
        }

        [Authorize]
        [HttpPost("new")]
        public async Task<ActionResult<Listing>> AddListing(Listing listing)
        {
            Console.WriteLine(listing.Make);
            listing = await _listingService.AddListing(listing);
            return Ok(listing);
        }

    }
}