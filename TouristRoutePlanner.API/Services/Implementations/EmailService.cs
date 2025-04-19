using Azure.Communication.Email;
using Azure;
using TouristRoutePlanner.API.Services.Interfaces;

namespace TouristRoutePlanner.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration["Azure:Communication:ConnectionString"];
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var client = new EmailClient(connectionString);

            var emailMessage = new EmailMessage(
                senderAddress: configuration["Email:SenderAddress"],
                content: new EmailContent("Reset Your Password")
                {
                    Html = $@"
                    <html>
                        <body>
                            <h2>Reset Your Password</h2>
                            <p>Your password reset code is:</p>
                            <h3 style='background-color: #f0f0f0; padding: 10px; font-family: monospace; text-align: center;'>{resetToken}</h3>
                            <p>Enter this code in the app to reset your password.</p>
                            <p>If you didn't request this, please ignore this email.</p>
                            <p>This code will expire in 15 minutes.</p>
                        </body>
                    </html>"
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(email) })
            );

            await client.SendAsync(WaitUntil.Completed, emailMessage);
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationToken)
        {
            var client = new EmailClient(connectionString);

            var emailMessage = new EmailMessage(
                senderAddress: configuration["Email:SenderAddress"],
                content: new EmailContent("Confirm Your Email")
                {
                    Html = $@"
                    <html>
                        <body>
                            y>
                            <h2>Welcome to Tourist Route Planner!</h2>
                            <p>Please confirm your email address by entering this code:</p>
                            <h3 style='background-color: #f0f0f0; padding: 10px; font-family: monospace; text-align: center;'>{confirmationToken}</h3>
                            <p>If you didn't create an account, please ignore this email.</p>
                        </body>
                    </html>"
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(email) })
            );
            await client.SendAsync(WaitUntil.Completed, emailMessage);
        }
    }
}
