using System;
using CBTClinic.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Questions
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }

        public Quizzes Quiz { get; set; }

        public ICollection<Option> Options { get; set; }
    }
}
