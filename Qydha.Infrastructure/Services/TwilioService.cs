using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Qydha.Infrastructure.Services;

public class SMSService : IMessageService
{
    private readonly TwilioSettings _twilio;
    public SMSService(IOptions<TwilioSettings> twilio)
    {
        _twilio = twilio.Value;
    }
    public async Task<Result> SendOtpAsync(string phoneNum, string username, string otp)
    {
        TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        var result = await MessageResource.CreateAsync(
            body: @$"مرحبا _*{username}*_ كود التحقق هو *{otp}*",
            from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
            to: phoneNum
        );
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            return Result.Fail(new()
            {
                Code = ErrorType.OTPPhoneSendingError,
                Message = $"can't send the SMS using Twilio with code =  {result.ErrorCode} , and Message = {result.ErrorMessage}"
            });
        }
        return Result.Ok();
    }
}
