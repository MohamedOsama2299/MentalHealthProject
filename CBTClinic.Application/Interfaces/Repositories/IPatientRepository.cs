using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Application.DTOs.Patient;
using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task<List<PatientListDto>> GetAllPatientsAsync();
        Task<bool> UpdateNameAsync(string userId, string newFullName);
        Task<bool> UpdateEmailAsync(string userId, string newEmail,UserManager<ApplicationUser> userManager);
        Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, UserManager<ApplicationUser> userManager);
        Task<bool> DeactivateAccountAsync(string userId);
        Task<int> CountAsync();



        Task<List<DoctorDto>> SearchDoctorsAsync(string patientId, DoctorSearchDto searchDto);
        Task AddFavoriteDoctorAsync(string patientId, int doctorId);
        Task RemoveFavoriteDoctorAsync(string patientId, int doctorId);
        Task<List<DoctorDto>> GetFavoriteDoctorsAsync(string patientId);



        Task<List<NotificationDto>> GetPatientsForAdminAsync();
        Task<bool> DeletePatientAsync(int patientId);

        Task<PatientListDto> GetByIdAsync(int id);

        Task AddNotificationAsync(Notification notification);
        Task<List<NotificationDto>> GetPatientNotificationsAsync(int patientId);
        Task MarkNotificationAsReadAsync(int notificationId);



        Task<int?> GetPatientIdByUserIdAsync(string userId);
        Task SendMessageAsync(Message message);
        Task<List<Message>> GetPatientMessagesAsync(int patientId);
        Task MarkMessageAsReadAsync(int messageId);



    }
}
