using CBTClinic.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Specialty is required")]
        public string Specialty { get; set; }

        public int YearsOfExperience { get; set; }
        public string WorkSchedule { get; set; }
        public int SessionDuration { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }

        public string? ImageUrl { get; set; }

        [JsonIgnore]
        public bool IsFavorite;



    }
}
