using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorRepository> _logger;

        public DoctorRepository(ApplicationDbContext context, ILogger<DoctorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            try
            {
                return await _context.Doctors.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all doctors");
                throw;
            }
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor with id {Id} not found", id);
                    return null;
                }
                return doctor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor by id {Id}", id);
                throw;
            }
        }

        public async Task AddAsync(Doctor doctor, IFormFile imageFile = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(doctor.FullName))
                    throw new ArgumentException("Doctor name cannot be empty");

                if (imageFile != null)
                {
                    doctor.ImageUrl = await SaveImageAsync(imageFile);
                }

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Doctor added successfully: {Name}", doctor.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding doctor");
                throw;
            }
        }




        public async Task UpdateAsync(Doctor doctor, IFormFile imageFile = null)
        {
            try
            {
                if (doctor == null) throw new ArgumentNullException(nameof(doctor));

                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(doctor.ImageUrl))
                    {
                        var oldPath = Path.Combine("wwwroot", doctor.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }

                    doctor.ImageUrl = await SaveImageAsync(imageFile);
                }

                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Doctor updated successfully: {Id}", doctor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor: {Id}", doctor?.Id);
                throw;
            }
        }



        public async Task DeleteAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor with id {Id} not found for deletion", id);
                    return;
                }

                if (!string.IsNullOrEmpty(doctor.ImageUrl))
                {
                    var oldPath = Path.Combine("wwwroot", doctor.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Doctor deleted successfully: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor: {Id}", id);
                throw;
            }
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
    }
}
