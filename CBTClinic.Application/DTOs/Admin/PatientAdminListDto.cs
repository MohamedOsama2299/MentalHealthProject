using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Appointment
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public string Status { get; set; }        // Active / Inactive
        public string Subscription { get; set; }  // Free / Pro
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
