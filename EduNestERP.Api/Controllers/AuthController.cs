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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var result = await _authService.LoginAsync(model.TenantId, model.UserId, model.Password);
            if (result == null) return Unauthorized();

            return Ok(result);
        }

        [HttpPost("first-reset")]
        public async Task<IActionResult> FirstReset([FromBody] FirstResetIn model)
        {
            var result = await _authService.FirstResetAsync(model.TenantId, model.UserId, model.NewPassword);
            if (!result) return NotFound();

            return Ok();
        }

    }
}