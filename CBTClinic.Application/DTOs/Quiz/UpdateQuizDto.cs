using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Quiz
{
    public class UpdateQuizDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsPaid { get; set; }

        public decimal Price { get; set; }
    }
}
