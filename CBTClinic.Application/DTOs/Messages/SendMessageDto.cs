using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CBTClinic.Application.DTOs.Messages
{
    public class SendMessageDto
    {
        public int PatientId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }


}
