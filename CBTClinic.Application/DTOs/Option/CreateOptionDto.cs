using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Option
{
    public class CreateOptionDto
    {
        public int QuestionId { get; set; }

        public string Text { get; set; }

        public int Score { get; set; }
    }
}
