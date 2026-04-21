using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Option
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int Score { get; set; }

        public int QuestionId { get; set; }

        public Questions Question { get; set; }
    }
}
