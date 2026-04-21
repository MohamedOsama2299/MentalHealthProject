using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class AdminReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }


        [Required]
        public int AdminId { get; set; }

        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
    }
}
