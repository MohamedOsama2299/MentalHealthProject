using CBTClinic.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> AdminLoginAsync(string email, string password);
        Task<bool> SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<bool> ResetPasswordAsync(string email, string newPassword);

    }
}
