using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CBTClinic.Infrastructure.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPatientRepository _patientRepository;

        public AdminProfileService(IAdminRepository adminRepository, IPatientRepository patientRepository, IHttpContextAccessor httpContextAccessor)
        {
            _adminRepository = adminRepository;
            _patientRepository = patientRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetAdminIdFromToken()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("AdminId");
            if (claim == null)
                throw new KeyNotFoundException("AdminId not found in token");
            return int.Parse(claim.Value);
        }



        public async Task<AdminProfileDto> GetProfileAsync()
        {
            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");

            return new AdminProfileDto
            {
                FullName = admin.FullName,
                Email = admin.Email,
                ImagePath = admin.ImagePath
            };
        }



        public async Task<AdminStatisticsDto> GetStatisticsAsync()
        {
            var totalPatients = await _patientRepository.CountAsync(); 
            return new AdminStatisticsDto
            {
                TotalUsers = totalPatients,    
                TotalProUsers = 0,               
                TotalFreeUsers = totalPatients   
            };
        }



        public async Task<(bool Success, string Message)> UpdateNameAsync(UpdateAdminNameDto dto)
        {
            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException();
            admin.FullName = dto.FullName;
            await _adminRepository.UpdateAsync(admin);
            return (true, "Name updated successfully");
        }



        public async Task<(bool Success, string Message)> UpdateEmailAsync(UpdateAdminEmailDto dto)
        {
            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException();
            admin.Email = dto.Email; 
            await _adminRepository.UpdateAsync(admin);
            return (true, "Email updated successfully");
        }



        public async Task<(bool Success, string Message)> ChangePasswordAsync(ChangeAdminPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return (false, "New password and confirmation do not match");

            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException();

            var hasher = new PasswordHasher<Admin>();
            var result = hasher.VerifyHashedPassword(admin, admin.PasswordHash, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Current password is incorrect");

            admin.PasswordHash = hasher.HashPassword(admin, dto.NewPassword);
            await _adminRepository.UpdateAsync(admin);

            return (true, "Password updated successfully");
        }



        public async Task<AuthResponseDto> AddOrUpdateImageAsync(UpdateAdminImageDto dto)
        {
            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException();

            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images", "admins");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                admin.ImagePath = $"/images/admins/{fileName}";
                await _adminRepository.UpdateAsync(admin);
            }

            return new AuthResponseDto
            {
                Token = "",
                UserId = admin.Id.ToString(),
                Email = admin.Email,
                FullName = admin.FullName,
                Message = "Image updated successfully",
                IsSuccess = true,
                ImagePath = admin.ImagePath 
            };
        }



        public async Task<(bool Success, string Message)> DeleteImageAsync()
        {
            int adminId = GetAdminIdFromToken();
            var admin = await _adminRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException();

            if (!string.IsNullOrEmpty(admin.ImagePath))
            {
                var fullPath = Path.Combine("wwwroot", admin.ImagePath.TrimStart('/'));
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }

            admin.ImagePath = null;
            await _adminRepository.UpdateAsync(admin);
            return (true, "Image deleted successfully");
        }
    }
}
