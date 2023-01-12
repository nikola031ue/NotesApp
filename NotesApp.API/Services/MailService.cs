using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;

namespace NotesApp.API.Services
{
    public class MailService// : IMailService
    {
        private static IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static async Task SendEmailAsync(string toMail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("nikola.pfc.kostic22@gmail.com", "JWT Auth");
            var to = new EmailAddress(toMail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

        }
    }
}
