using CBTClinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public  string FullName { get; set; }
        public  string Email { get; set; }
        public bool IsActive { get; set; }
       

        // Favorite Doctors navigation
        public ICollection<PatientFavoriteDoctor> FavoriteDoctors { get; set; } = new List<PatientFavoriteDoctor>();
    }
}

