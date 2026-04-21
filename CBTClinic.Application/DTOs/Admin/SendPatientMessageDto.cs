using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Appointment
{
    public class SendPatientMessageDto
    {
        public string Email { get; set; }     // auto filled
        public string Subject { get; set; }
        public string Message { get; set; }
    }

}
