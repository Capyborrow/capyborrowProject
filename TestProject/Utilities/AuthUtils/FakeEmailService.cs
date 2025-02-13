using capyborrowProject.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utilities.AuthUtils
{
    internal class FakeEmailService : EmailService
    {
        public FakeEmailService() : base(new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "AzureEmailCommunicationService:ConnectionString", "fake-connection-string" },
            { "AzureEmailCommunicationService:SenderEmail", "noreply@example.com" }
        }).Build())
        { }

        public override Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            Console.WriteLine($"Fake email sent to {recipientEmail} with subject '{subject}'");
            return Task.CompletedTask;
        }
    }
}
