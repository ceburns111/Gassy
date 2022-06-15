using Gassy.Models;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("agent")]
    public class AgentController : ControllerBase
    {
        private IAgentService _agentService;

        public AgentController(IAgentService agentService)
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
    }
}