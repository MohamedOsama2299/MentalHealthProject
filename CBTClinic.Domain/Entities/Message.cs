using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; }
    }

}
