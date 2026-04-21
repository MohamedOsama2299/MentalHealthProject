using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CBTClinic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(LoginDto dto)
        {
            var result = await _authService.AdminLoginAsync(dto.Email, dto.Password);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var sent = await _authService.SendOtpAsync(email);
            if (!sent) return BadRequest("Email not found");

            return Ok("OTP sent");
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var ok = await _authService.VerifyOtpAsync(dto.Email, dto.Otp);

            if (!ok) return BadRequest("Invalid OTP");

            return Ok("OTP Verified");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(model.Email, model.NewPassword);

            if (!result)
                return BadRequest(new { message = "Failed to reset password" });

            return Ok(new { message = "Password reset successfully" });
        }




    }
}
