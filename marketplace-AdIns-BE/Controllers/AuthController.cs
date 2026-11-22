using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse? result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { status = "error", message = "Email/password salah atau akun tidak aktif." });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            RegisterResponse? result = await _authService.RegisterAsync(request);
            if (result == null)
            {
                if (request.Role == "Buyer")
                {
                    return BadRequest(new { status = "error", message = "Email ini sudah terdaftar seabagai Buyer." });
                } else if (request.Role == "Seller")
                {
                    return BadRequest(new { status = "error", message = "Email ini sudah terdaftar seabagai Seller." });
                }
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}