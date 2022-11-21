using MailKit.Net.Smtp;
using MailKit.Security;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace MarketPlace.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly MailSettingsDTO _mailSettings;

        public EmailService(IOptions<MailSettingsDTO> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendVerificationEmail(string email, string activationCode)
        {
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verifivation Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your Verification Code is: {activationCode}"
            };

            var client = new SmtpClient();

            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();
        }

        public async Task SendUserPasswordEmail(string email, string password)
        {
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verifivation Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your Password is: {password}"
            };

            var client = new SmtpClient();

            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
