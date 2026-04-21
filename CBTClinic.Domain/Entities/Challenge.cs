using CBTClinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class Challenge
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Steps { get; set; }

        public string Benefits { get; set; }

        public ChallengeDifficulty Difficulty { get; set; }

        public int DurationDays { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
