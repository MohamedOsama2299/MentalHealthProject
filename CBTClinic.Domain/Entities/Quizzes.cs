using System;
using CBTClinic.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Quizzes
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsPaid { get; set; } 

        public decimal Price { get; set; } 

        public ICollection<Questions> Questions { get; set; }
    }
}
