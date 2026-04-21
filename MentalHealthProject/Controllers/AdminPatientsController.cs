using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Messages;
using CBTClinic.Application.DTOs.Notification;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CBTClinic.API.Controllers
{

    [ApiController]
    [Route("api/admin/patients")]
    [Authorize(Roles = "Admin")]
    public class AdminPatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IEmailService _emailService;

        public AdminPatientsController(
            IPatientRepository patientRepository,
            IEmailService emailService)
        {
            _patientRepository = patientRepository;
            _emailService = emailService;
        }



        // GET: api/admin/patients
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientRepository.GetPatientsForAdminAsync();

            return Ok(new
            {
                message = "Patients fetched successfully",
                count = patients.Count,
                data = patients
            });
        }



        // DELETE: api/admin/patients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientRepository.DeletePatientAsync(id);

            if (!result)
                return NotFound(new { message = "Patient not found" });

            return Ok(new { message = "Patient deleted successfully" });
        }


        // POST: api/admin/patients/send-message
        [HttpPost("send-message")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var message = new Message
            {
                PatientId = dto.PatientId,
                Title = dto.Title,
                Content = dto.Content
            };

            await _patientRepository.SendMessageAsync(message);

            return Ok(new { message = "Message sent successfully" });
        }





        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                PatientId = dto.PatientId,
                Title = dto.Title,
                Message = dto.Message
            };

            await _patientRepository.AddNotificationAsync(notification);

            return Ok(new { message = "Notification sent successfully" });
        }




    }

}
