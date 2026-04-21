using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public  Patient Patient { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }

        //public string? ImageUrl { get; set; }


    }
}
