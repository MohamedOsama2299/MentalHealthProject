using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Podcast
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
