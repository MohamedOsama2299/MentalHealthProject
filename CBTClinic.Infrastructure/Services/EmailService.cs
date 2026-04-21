using CBTClinic.Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string message)
        {
            var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = false;

            mail.From = new MailAddress("your_email@gmail.com");

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your_email@gmail.com", "your_app_password"),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
