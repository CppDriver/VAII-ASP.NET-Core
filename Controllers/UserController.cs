using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.UserService;

namespace MultimediaLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getUserData")]
        public async Task<IActionResult> GetUserData([FromQuery] string username)
        {
            var result = await _userService.GetUser(username);
            return Ok(result);
        }

        [HttpGet("getUserDataFull")]
        public async Task<IActionResult> GetUserDataFull([FromQuery] string username)
        {
            var result = await _userService.GetUserDataFull(username);
            return Ok(result);
        }

        [HttpPut("updateUserData")]
        public async Task<IActionResult> UpdateUserData([FromBody] UserData userData)
        {
            var result = await _userService.UpdateUserData(userData);
            if (result == false)
                return StatusCode(StatusCodes.Status400BadRequest);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("updateProfileImg")]
        public async Task<IActionResult> UpdateProfileImg(IFormFile file)
        {
            var result = await _userService.UpdateProfileImg(file);
            if (result.Equals("OK"))
                return Ok();
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [HttpGet("getUserProfileImg/{userId}")]
        public async Task<IActionResult> GetUserProfileImg(ulong userId)
        {
            var result = await _userService.GetUserProfileImg(userId);
            return File(result, "image/jpeg");
        }

        [HttpGet("getUserGalleries/{username}")]
        public async Task<IActionResult> GetUserGalleries(string username)
        {
            var result = await _userService.GetUserGalleries(username);
            return Ok(result);
        }
    }
}
