using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WebApplication.Services
{
    public class EmailSettings
    {
        public string FromName { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");
            var smtpFrom = Environment.GetEnvironmentVariable("SMTP_FROM");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpFrom) ||
                string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
                return;

            _ = int.TryParse(smtpPort, out var port);

            using var client = new SmtpClient(smtpHost, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            var message = new MailMessage
            {
                From = new MailAddress(smtpFrom, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }
    }
}
