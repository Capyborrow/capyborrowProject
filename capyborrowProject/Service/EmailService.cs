using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace capyborrowProject.Service
{
    public class EmailService
    {
        private readonly string _connectionString;
        private readonly string _senderEmail;

        public EmailService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureEmailCommunicationService:ConnectionString"];
            _senderEmail = configuration["AzureEmailCommunicationService:SenderEmail"];
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var emailClient = new EmailClient(_connectionString);

            var emailMessage = new EmailMessage(
                _senderEmail,
                recipientEmail,
                new EmailContent(subject) { Html = body }
            );

            await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
        }
    }
}
