using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

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
            var smtpFrom = Environment.GetEnvironmentVariable("SMTP_FROM");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpFrom) ||
                string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
                return;

            _ = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port);
            if (port == 0) port = 587;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, smtpFrom));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);
        }
    }
}
