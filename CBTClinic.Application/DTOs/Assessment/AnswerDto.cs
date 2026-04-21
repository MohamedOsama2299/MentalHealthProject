using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Assessment
{
    public class AnswerDto
    {
        public int QuestionId { get; set; }

        public int SelectedOptionId { get; set; }
    }
}
