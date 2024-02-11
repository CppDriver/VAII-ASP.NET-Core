using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Services.AuthService;

namespace MultimediaLibrary.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var result = await _authService.Login(username, password);

            if (result.Succeeded)
            {
                return Ok(new { token = _authService.GenerateToken(username), username });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register()
        {
            var email = Request.Form["email"];
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var result = await _authService.Register(email, username, password);

            if (result.Succeeded)
            {
                return Ok(new { msg = "registered" });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            var username = Request.Form["username"];
            var oldPassword = Request.Form["oldPassword"];
            var newPassword = Request.Form["newPassword"];
            
            var result = await _authService.ChangePassword(username, oldPassword, newPassword);

            if (result.Succeeded)
            {
                return Ok(new { msg = "Password changed"});
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [Authorize]
        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var result = await _authService.DeleteAccount(username, password);

            if (result.Succeeded)
            {
                return Ok(new { msg = "Account deleted"});
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
