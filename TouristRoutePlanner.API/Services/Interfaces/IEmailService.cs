namespace TouristRoutePlanner.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetToken);
        Task SendEmailConfirmationAsync(string email, string confirmationToken);
    }
}
