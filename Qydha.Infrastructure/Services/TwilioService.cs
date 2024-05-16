using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Qydha.Infrastructure.Services;

public class SMSService(IOptions<TwilioSettings> twilio) : IMessageService
{
    private readonly TwilioSettings _twilio = twilio.Value;

    public async Task<Result<string>> SendOtpAsync(string phoneNum, string username, string otp)
    {
        TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        var result = await MessageResource.CreateAsync(
            body: @$"مرحبا _*{username}*_ كود التحقق هو *{otp}*",
            from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
            to: phoneNum
        );
        if (!string.IsNullOrEmpty(result.ErrorMessage))
            return Result.Fail(new OtpPhoneSendingError("SMS_Twilio"));

        return Result.Ok($"SMS:Twilio:{_twilio.TwilioPhoneNumber}");
    }
}
