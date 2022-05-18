using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListingController : ControllerBase
    {
        private IAgentService _agentService;
        private IListingService _listingService;    

        public ListingController(IAgentService agentService, IListingService listingService)
        {
            _agentService = agentService;
            _listingService = listingService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _agentService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "AgentName or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetListings")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetListing()
        {
            var listings = await _listingService.GetListings();
            return Ok(listings);
        }

        [Authorize]
        [HttpPost("AddListing")]
        public async Task<ActionResult<Listing>> AddListing(Listing listing)
        {
            Console.WriteLine(listing.Make);
            listing = await _listingService.AddListing(listing);
            return Ok(listing);
        }

    }
}