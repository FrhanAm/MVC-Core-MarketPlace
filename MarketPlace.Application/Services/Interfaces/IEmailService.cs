namespace MarketPlace.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string activationCode);
        Task SendUserPasswordEmail(string email, string password);
    }
}
