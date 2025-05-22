using Microsoft.Extensions.Configuration;
using OdontofastAPI.Service.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace OdontofastAPI.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string? _apiKey;

        public EmailService(IConfiguration config)
        {
            _apiKey = config["ApiKeySendGridEmail"]; // ApiKey encontra-se em appsettings.json ou secrets
        }

        public async Task EnviarEmailMotivacionalAsync(string destinatario, string mensagem)
        {
            Console.WriteLine($"Tentando enviar email para: {destinatario}");

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("sousaradev@gmail.com", "Odontofast");
            var subject = "Você está mandando bem!";
            var to = new EmailAddress(destinatario);
            var plainTextContent = mensagem;
            var htmlContent = $"<strong>{mensagem}</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
