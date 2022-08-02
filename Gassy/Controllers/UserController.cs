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
            var response = await _userService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken()
        {
            try {
                var refreshToken = Request.Cookies["refreshToken"];
                var response = await _userService.RefreshToken(refreshToken, IpAddress());
                SetTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        [HttpPost("Revoke-Token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            await _userService.RevokeToken(token, IpAddress());
            return Ok(new { message = "Token revoked" });
        }
        
        [HttpGet("{id}/Refresh-Tokens")]
        public async Task<ActionResult<IEnumerable<RefreshToken>>> GetRefreshTokens(int id)
        {
            Console.WriteLine("Getting refresh tokens");
            var refreshTokens = await _userService.GetRefreshTokensByUserId(id);
            return Ok(refreshTokens);
        }

        [AllowAnonymous]
        [HttpPost("Signup")]
        public async Task<ActionResult<UserDTO>> Signup(UserDTO newUser)
        {
            var response = await _userService.AddUser(newUser);
            return Ok(response);
        }

        [HttpPost("Edit")]
        public async Task<ActionResult<EditUserDTO>> Edit(EditUserDTO newUser)
        {
            var response = await _userService.EditUser(newUser);
            return Ok(response);
        }

        
        [HttpPost("{id}/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUser(id);
            return Ok("User deleted");
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

        // helper methods

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }


     
    }
}