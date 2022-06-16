using Gassy.Models;
using Gassy.Services;
using Gassy.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Gassy.Models.Users; 
using Gassy.Entities; 

namespace Gassy.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Users")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            Console.WriteLine("Authenticating...");
            var response = await _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "UserName or password is incorrect" });

            return Ok(response);
        }


        [Authorize(RoleId.Admin)]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            Console.WriteLine("Querying Users...");
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            Console.WriteLine($"Querying Users for Id:{id}...");

            // only admins can access other user records
            var currentUser = (User)HttpContext.Items["User"];
            if (id != currentUser.Id && currentUser.RoleId != RoleId.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var user =  _userService.GetById(id);
            return Ok(user);
        }
    }
}