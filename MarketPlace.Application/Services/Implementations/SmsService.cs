using MarketPlace.Application.Services.Interfaces;

namespace MarketPlace.Application.Services.Implementations;

public class SmsService : ISmsService
{
    private string apiKey = "Your Api Key";
    public async Task SendVerificationSms(string mobile, string activationCode)
    {
        Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(apiKey);
        await api.VerifyLookup(mobile, activationCode, "TemplateName");
    }

    public async Task SendUserPasswordSms(string mobile, string password)
    {
        Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(apiKey);
        await api.VerifyLookup(mobile, password, "TemplateName");
    }
}
