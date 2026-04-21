using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class PatientAnswers
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int QuestionId { get; set; }

        public int SelectedOptionId { get; set; }
        public DateTime AnsweredAt { get; set; }
    }
}
