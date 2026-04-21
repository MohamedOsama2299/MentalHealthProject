using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBTClinic.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;

        private static Dictionary<string, string> _otpStore = new();

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            ApplicationDbContext context,
            IJwtTokenService jwtTokenService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
        }

        //  USER LOGIN 
        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDto
                {
                    Token = "",
                    UserId = "",
                    Email = "",
                    FullName = "",
                    Message = "User not found",
                    IsSuccess = false
                };

            var check = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!check)
                return new AuthResponseDto
                {
                    Token = "",
                    UserId = "",
                    Email = "",
                    FullName = "",
                    Message = "Invalid password",
                    IsSuccess = false
                };

            var token = _jwtTokenService.GenerateTokenFromUser(user.Id, user.Email);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Message = "Login successful",
                IsSuccess = true
            };
        }

        //  REGISTER
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(" , ", result.Errors.Select(e => e.Description)));

            var patient = new Patient
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                IsActive = true
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var token = _jwtTokenService.GenerateTokenFromUser(user.Id, user.Email);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Message = "Registration successful",
                IsSuccess = true
            };
        }



        // ADMIN LOGIN
        public async Task<AuthResponseDto> AdminLoginAsync(string email, string password)
        {
            // جلب الأدمن من قاعدة البيانات حسب الايميل
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null)
                return new AuthResponseDto
                {
                    Token = "",
                    UserId = "",
                    Email = "",
                    FullName = "",
                    Message = "Invalid Admin Login",
                    IsSuccess = false
                };

            // تحقق من الباسورد باستخدام PasswordHasher
            var hasher = new PasswordHasher<Admin>();
            var result = hasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return new AuthResponseDto
                {
                    Token = "",
                    UserId = "",
                    Email = "",
                    FullName = "",
                    Message = "Invalid Admin Login",
                    IsSuccess = false
                };

            // إنشاء Claims للأدمن
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, admin.FullName),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim("AdminId", admin.Id.ToString())
    };

            // توليد JWT
            var token = _jwtTokenService.GenerateAdminToken(claims);

            return new AuthResponseDto
            {
                Token = token,
                UserId = admin.Id.ToString(),
                Email = admin.Email,
                FullName = admin.FullName,
                Message = "Admin logged in successfully",
                IsSuccess = true
            };
        }


        // RESET PASSWORD 
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            return result.Succeeded;
        }

        //  OTP 
        public async Task<bool> SendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            _otpStore[email] = otp;

            var body = $"<h2>Your OTP Code</h2><p>Your verification code is: <b>{otp}</b></p>";
            await _emailService.SendEmailAsync(email, "Password Reset OTP", body);

            return true;
        }

        public Task<bool> VerifyOtpAsync(string email, string otp)
        {
            if (_otpStore.ContainsKey(email) && _otpStore[email] == otp)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }
    }
}
