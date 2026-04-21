using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IAdminProfileService
    {
        Task<AdminProfileDto> GetProfileAsync();
        Task<AdminStatisticsDto> GetStatisticsAsync();
        Task<(bool Success, string Message)> UpdateNameAsync(UpdateAdminNameDto dto);
        Task<(bool Success, string Message)> UpdateEmailAsync(UpdateAdminEmailDto dto);
        Task<(bool Success, string Message)> ChangePasswordAsync(ChangeAdminPasswordDto dto);
        Task<AuthResponseDto> AddOrUpdateImageAsync(UpdateAdminImageDto dto);
        Task<(bool Success, string Message)> DeleteImageAsync();
    }

}
