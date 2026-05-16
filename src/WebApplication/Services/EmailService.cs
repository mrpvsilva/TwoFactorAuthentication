using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Resend;

namespace WebApplication.Services
{
    public class EmailSettings
    {
        public string From { get; set; }
        public string FromName { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly EmailSettings _settings;

        public EmailService(IResend resend, IOptions<EmailSettings> options)
        {
            _resend = resend;
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RESEND_API_KEY")))
                return;

            var message = new EmailMessage();
            message.From = $"{_settings.FromName} <{_settings.From}>";
            message.To.Add(to);
            message.Subject = subject;
            message.HtmlBody = body;

            await _resend.EmailSendAsync(message);
        }
    }
}
