using CBTClinic.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Content
{
    public class UpdateChallengeDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Steps { get; set; }
        public string? Benefits { get; set; }

        public ChallengeDifficulty? Difficulty { get; set; }
        public int? DurationDays { get; set; }
    }
}
