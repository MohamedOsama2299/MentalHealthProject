using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Appointment
{
    public class SubmitQuizDto
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
