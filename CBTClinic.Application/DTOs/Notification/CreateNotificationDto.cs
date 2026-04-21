using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Notification
{
    public class CreateNotificationDto
    {
        public int PatientId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

}
