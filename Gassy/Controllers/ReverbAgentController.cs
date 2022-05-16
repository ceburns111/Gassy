using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReverbAgentController : ControllerBase
    {
        private IAgentService _agentService;

        public ReverbAgentController(IAgentService agentService)
        {
            _agentService = agentService;
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
            var listings = await _agentService.GetListings();
            return Ok(listings);
        }

        [Authorize]
        [HttpPost("AddListing")]
        public async Task<ActionResult<Listing>> AddListing(Listing listing)
        {
            Console.WriteLine(listing.Make);
            listing = await _agentService.AddListing(listing);
            return Ok(listing);
        }

    }
}