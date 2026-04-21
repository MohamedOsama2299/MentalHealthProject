using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Application.DTOs.Content
{
    public class CreatePodcastDto
    {
        public string Title { get; set; }

        public IFormFile File { get; set; }
    }
}
