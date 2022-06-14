using Gassy.Models;
using Gassy.Helpers;
using Gassy.Services;
using Microsoft.AspNetCore.Mvc;


namespace Gassy.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateUserRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "UserName or password is incorrect" });

            return Ok(response);
        }
    }
}