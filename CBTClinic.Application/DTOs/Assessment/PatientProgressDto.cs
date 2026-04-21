using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Assessment.Patient
{
	public class PatientProgressDto
	{
		public int QuizId { get; set; }

		public int TotalScore { get; set; }

		public string SeverityLevel { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
