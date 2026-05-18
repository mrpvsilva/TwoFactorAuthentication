using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication.Services
{
    public class EmailSettings
    {
        [Required]
        public string FromName { get; set; }

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string SenderEmail { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly HttpClient _http;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IHttpClientFactory httpClientFactory, IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _http = httpClientFactory.CreateClient();
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var payload = new
            {
                sender = new { name = _settings.FromName, email = _settings.SenderEmail },
                to = new[] { new { email = to } },
                subject,
                htmlContent = body
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("api-key", _settings.ApiKey);
            request.Headers.Add("accept", "application/json");

            try
            {
                _logger.LogInformation("Enviando e-mail para {To} com assunto '{Subject}'.", to, subject);

                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("E-mail enviado com sucesso para {To}. Status: {StatusCode}.", to, (int)response.StatusCode);
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Falha ao enviar e-mail para {To}. Status: {StatusCode}. Resposta: {ResponseBody}.",
                        to, (int)response.StatusCode, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao tentar enviar e-mail para {To}.", to);
            }
        }
    }
}
