using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Assessment
{
    public class AssessmentResultDto
    {
        public int TotalScore { get; set; }

        public string SeverityLevel { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
