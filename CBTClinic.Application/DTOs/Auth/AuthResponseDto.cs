using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public  string Message { get; set; }
        public  bool IsSuccess { get; set; }
        public  string FullName { get; set; }
        public object ImagePath { get; set; }
        public object Success { get; set; }
    }
}
