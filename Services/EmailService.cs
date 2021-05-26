using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV_Chatbot.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<bool> SendEmail(string title, string body)
        {
            var apiKey = _config.GetValue<string>("SendGrid");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("juancruztrinidaddeveloper@gmail.com", "Contacto"),
                Subject = title,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress("juancruztrinidad97@gmail.com"));
            var response = await client.SendEmailAsync(msg);
            string algo = response.Body.ReadAsStringAsync().Result;
            Console.WriteLine(response.Body.ReadAsStringAsync().Result); // The message will be here
            return response.IsSuccessStatusCode;
        }
    }
}
