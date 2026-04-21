using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialty { get; set; }
        public int YearsOfExperience { get; set; }
        public string WorkSchedule { get; set; }
        public int SessionDuration { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public string? ImageUrl { get; set; }

        // Patients who favorited this doctor
        public ICollection<PatientFavoriteDoctor> PatientsWhoFavorited { get; set; } = new List<PatientFavoriteDoctor>();
    }
}

