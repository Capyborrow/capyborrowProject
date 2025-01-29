using Azure;
using Azure.Communication.Email;
using System.Threading.Tasks;

namespace capyborrowProject.Service
{
    public class EmailService
    {
        private readonly string _connectionString = "endpoint=https://eunicommunicationservice.unitedstates.communication.azure.com/;accesskey=Co27d5aSXMGE7dHUF8d2Xiz2WZ8b48wwfwuAFkiIhMNamyS8JjIsJQQJ99BAACULyCp3WYzYAAAAAZCSUNQB";
        private readonly string _senderEmail = "DoNotReply@bf4075ce-71ea-4d37-8f73-591a1f5bb794.azurecomm.net"; // Use your verified sender email

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