using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class EmailSender : IEmailSender
    {
        public string BrevoKey { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            BrevoKey = configuration.GetValue<string>("Brevo:SecretKey");
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Configuration.Default.ApiKey.Add("api-key", BrevoKey);

            var apiInstance = new TransactionalEmailsApi();
            var sender = new SendSmtpEmailSender("Expert Offers", "coder.prob12348@gmail.com");
            var toList = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(email) };

            var emailData = new SendSmtpEmail(
                sender: sender,
                to: toList,
                subject: subject,
                htmlContent: htmlMessage
            );

            try
            {
                var result = apiInstance.SendTransacEmail(emailData);
                Debug.WriteLine(result.ToJson());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error sending email: {e.Message}");
                throw;
            }
        }
    }
}
