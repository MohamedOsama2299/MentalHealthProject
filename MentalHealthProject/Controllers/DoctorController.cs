using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace CBTClinic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        // GET: api/doctor
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            if (doctors == null || !doctors.Any())
                return NotFound(new { message = "No doctors found in the database." });

            var dtos = doctors.Select(d => new DoctorDto
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
                ImageUrl = string.IsNullOrEmpty(d.ImageUrl) ? null : $"{Request!.Scheme}://{Request!.Host}/images/doctors/{d.ImageUrl}"
            }).ToList();

            return Ok(new { message = "Doctors fetched successfully.", count = dtos.Count, data = dtos });
        }





        // GET: api/doctor/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = $"Doctor with id {id} not found." });

            var dto = new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialty = doctor.Specialty,
                YearsOfExperience = doctor.YearsOfExperience,
                WorkSchedule = doctor.WorkSchedule,
                SessionDuration = doctor.SessionDuration,
                Location = doctor.Location,
                Bio = doctor.Bio,
                ImageUrl = string.IsNullOrEmpty(doctor.ImageUrl) ? null : $"{Request!.Scheme}://{Request!.Host}/images/doctors/{doctor.ImageUrl}"
            };

            return Ok(new { message = "Doctor fetched successfully.", data = dto });
        }





        // POST: api/doctor
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] DoctorDto doctorDto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var doctor = new Doctor
            {
                FullName = doctorDto.FullName,
                Email = doctorDto.Email,
                Specialty = doctorDto.Specialty,
                YearsOfExperience = doctorDto.YearsOfExperience,
                WorkSchedule = doctorDto.WorkSchedule,
                SessionDuration = doctorDto.SessionDuration,
                Location = doctorDto.Location,
                Bio = doctorDto.Bio,
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine("wwwroot/images/doctors", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                doctor.ImageUrl = $"https://localhost:7121/images/doctors/{fileName}";
            }

            await _doctorRepository.AddAsync(doctor);

            var resultDto = new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialty = doctor.Specialty,
                YearsOfExperience = doctor.YearsOfExperience,
                WorkSchedule = doctor.WorkSchedule,
                SessionDuration = doctor.SessionDuration,
                Location = doctor.Location,
                Bio = doctor.Bio,
                ImageUrl = doctor.ImageUrl
            };

            return Ok(new { message = "Doctor added successfully", data = resultDto });
        }




        // PUT: api/doctor/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] DoctorDto doctorDto, IFormFile? imageFile)
        {
            var existingDoctor = await _doctorRepository.GetByIdAsync(id);
            if (existingDoctor == null)
                return NotFound(new { error = $"Doctor with id {id} not found." });

            existingDoctor.FullName = doctorDto.FullName;
            existingDoctor.Email = doctorDto.Email;
            existingDoctor.Specialty = doctorDto.Specialty;
            existingDoctor.YearsOfExperience = doctorDto.YearsOfExperience;
            existingDoctor.WorkSchedule = doctorDto.WorkSchedule;
            existingDoctor.SessionDuration = doctorDto.SessionDuration;
            existingDoctor.Location = doctorDto.Location;
            existingDoctor.Bio = doctorDto.Bio;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingDoctor.ImageUrl))
                {
                    var oldFile = Path.Combine("wwwroot/images/doctors", Path.GetFileName(existingDoctor.ImageUrl));
                    if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
                }

                var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine("wwwroot/images/doctors", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingDoctor.ImageUrl = $"https://localhost:7121/images/doctors/{fileName}";
            }

            await _doctorRepository.UpdateAsync(existingDoctor);

            return Ok(new { message = "Doctor updated successfully", data = doctorDto });
        }





        // DELETE: api/doctor/{id}/image
        [HttpDelete("{id}/image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = $"Doctor with id {id} not found." });

            doctor.ImageUrl = null; 
            await _doctorRepository.UpdateAsync(doctor);

            return Ok(new { message = $"Image for doctor {id} deleted successfully." });
        }




        // DELETE: api/doctor/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound(new { error = "Doctor not found" });

            await _doctorRepository.DeleteAsync(id);
            return Ok(new { message = "Doctor deleted successfully" });
        }




        // SEARCH: api/doctor/search
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] DoctorSearchDto searchDto)
        {
            var doctors = await _doctorRepository.SearchDoctorsAsync(null, searchDto);

            if (!doctors.Any())
                return NotFound(new { error = "No doctors matched your search criteria." });

            return Ok(new
            {
                message = "Doctors fetched successfully",
                data = doctors
            });
        }








    }
}
