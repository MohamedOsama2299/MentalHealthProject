using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Appointment
{
    public class UpdateAdminNameDto
    {
        [Required]
        public string FullName { get; set; }
    }

    public class UpdateAdminEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ChangeAdminPasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match.")]
        public string ConfirmNewPassword { get; set; }
    }

    public class UpdateAdminImageDto
    {
        public IFormFile? Image { get; set; }
    }

    public class AdminProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
    }

    public class AdminStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalFreeUsers { get; set; }
        public int TotalProUsers { get; set; }
    }
}
