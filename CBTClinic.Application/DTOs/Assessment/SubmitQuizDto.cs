using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Assessment
{
    public class SubmitQuizDto
    {
        public int QuizId { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }
}
