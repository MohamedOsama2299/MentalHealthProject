using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Application.DTOs.Patient;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Repositories
{


    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PatientListDto>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,
                    IsActive = p.IsActive

                })
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<bool> UpdateNameAsync(string userId, string newFullName)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return false;
            patient.FullName = newFullName;
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> UpdateEmailAsync(
               string userId,
               string newEmail,
               UserManager<ApplicationUser> userManager)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return false;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            patient.Email = newEmail;

            user.Email = newEmail;
            user.UserName = newEmail;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, UserManager<ApplicationUser> userManager)
        {
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }


        public async Task<bool> DeactivateAccountAsync(string userId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return false;
            patient.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<int> CountAsync()
        {
            return await _context.Patients.CountAsync();
        }


        public async Task<List<DoctorDto>> SearchDoctorsAsync(string patientId, DoctorSearchDto searchDto)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.FullName))
                query = query.Where(d => d.FullName.Contains(searchDto.FullName));

            if (!string.IsNullOrWhiteSpace(searchDto.Specialty))
                query = query.Where(d => d.Specialty == searchDto.Specialty);

            var favoriteDoctorIds = await _context.PatientFavoriteDoctors
                .Where(f => f.PatientId == patientId)
                .Select(f => f.DoctorId)
                .ToListAsync();

            return await query
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Email = d.Email,
                    Specialty = d.Specialty,
                    YearsOfExperience = d.YearsOfExperience,
                    WorkSchedule = d.WorkSchedule,
                    SessionDuration = d.SessionDuration,
                    Location = d.Location,
                    Bio = d.Bio,
                    ImageUrl = d.ImageUrl,
                    IsFavorite = favoriteDoctorIds.Contains(d.Id)
                })
                .AsNoTracking()
                .ToListAsync();
        }




        public async Task AddFavoriteDoctorAsync(string patientId, int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with id {doctorId} does not exist.");

            var alreadyFavorite = await _context.PatientFavoriteDoctors
                .AnyAsync(f => f.PatientId == patientId && f.DoctorId == doctorId);

            if (alreadyFavorite)
                throw new InvalidOperationException($"Doctor {doctorId} is already in favorites.");

            _context.PatientFavoriteDoctors.Add(new PatientFavoriteDoctor
            {
                PatientId = patientId,
                DoctorId = doctorId
            });

            await _context.SaveChangesAsync();
        }



        public async Task RemoveFavoriteDoctorAsync(string patientId, int doctorId)
        {
            var favorite = await _context.PatientFavoriteDoctors
                .FirstOrDefaultAsync(f => f.PatientId == patientId && f.DoctorId == doctorId);

            if (favorite == null)
                throw new KeyNotFoundException($"Doctor {doctorId} is not in your favorites.");

            _context.PatientFavoriteDoctors.Remove(favorite);
            await _context.SaveChangesAsync();
        }



        public async Task<List<DoctorDto>> GetFavoriteDoctorsAsync(string patientId)
        {
            var favoriteDoctorIds = await _context.PatientFavoriteDoctors
                .Where(f => f.PatientId == patientId)
                .Select(f => f.DoctorId)
                .ToListAsync();

            if (!favoriteDoctorIds.Any())
                return new List<DoctorDto>();

            var existingDoctors = await _context.Doctors
                .Where(d => favoriteDoctorIds.Contains(d.Id))
                .AsNoTracking()
                .ToListAsync();

            return existingDoctors
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specialty = d.Specialty,
                    YearsOfExperience = d.YearsOfExperience,
                    WorkSchedule = d.WorkSchedule,
                    SessionDuration = d.SessionDuration,
                    Location = d.Location,
                    Bio = d.Bio,
                    ImageUrl = d.ImageUrl,
                    IsFavorite = true
                })
                .ToList();
        }




        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine("wwwroot", "images", "doctors");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/doctors/{fileName}";
        }




        public async Task<List<NotificationDto>> GetPatientsForAdminAsync()
        {
            var patients = await _context.Patients
                .Select(p => new NotificationDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,

                    Status = p.IsActive ? "Active" : "Inactive",

                    Subscription = _context.Subscriptions
                        .Where(s => s.UserId == p.UserId && s.IsActive)
                        .Select(s => s.PlanType)
                        .FirstOrDefault() ?? "Free",

                    CreatedAt = p.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();

            return patients;
        }



        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<PatientListDto?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Where(p => p.Id == id)
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();
        }



        public async Task<List<NotificationDto>> GetPatientNotificationsAsync(int patientId)
        {
            return await _context.Notifications
                .Where(n => n.PatientId == patientId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }


        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }



        public async Task<int?> GetPatientIdByUserIdAsync(string userId)
        {
            return await _context.Patients
                .Where(p => p.UserId == userId)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task SendMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetPatientMessagesAsync(int patientId)
        {
            return await _context.Messages
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }


    }
}

