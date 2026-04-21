using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetAllAsync();
        Task<Doctor> GetByIdAsync(int id);
        Task AddAsync(Doctor doctor, IFormFile imageFile = null);
        Task UpdateAsync(Doctor doctor, IFormFile imageFile = null);
        Task DeleteAsync(int id);

        Task<List<DoctorDto>> SearchDoctorsAsync(string patientId, DoctorSearchDto searchDto);
       
    }
}
