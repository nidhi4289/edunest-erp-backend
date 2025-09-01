using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduNestERP.Api.Model;
using EduNestERP.Application.Interfaces;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { success = false, message = "UserId and NewPassword are required." });

            var result = await _userService.ResetPasswordAsync(request.UserId, request.NewPassword);
            if (result)
                return Ok(new { success = true, message = "Password reset successful." });
            else
                return NotFound(new { success = false, message = "User not found or password reset failed." });
        }

    }
}