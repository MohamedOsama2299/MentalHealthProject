using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Auth
{
    public class VerifyOtpDto
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
    }
}
