using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Question
{
    public class CreateQuestionDto
    {
        public int QuizId { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }
    }
}
