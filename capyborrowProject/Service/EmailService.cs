using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace capyborrowProject.Service
{
    public class EmailService(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration["AzureEmailCommunicationService:ConnectionString"]!;
        private readonly string _senderEmail = configuration["AzureEmailCommunicationService:SenderEmail"]!;

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
