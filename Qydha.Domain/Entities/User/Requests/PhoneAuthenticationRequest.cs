
namespace Qydha.Domain.Entities;

public class PhoneAuthenticationRequest
{
    public Guid Id { get; set; }
    public string Otp { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTimeOffset? UsedAt { get; set; } = null;
    public string SentBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public PhoneAuthenticationRequest() { }
    public PhoneAuthenticationRequest(Guid userId, string otp, string sender)
    {
        UserId = userId;
        Otp = otp;
        CreatedAt = DateTimeOffset.UtcNow;
        SentBy = sender;
    }
    public Result IsValidToUpdatePasswordUsingIt(Guid userId)
    {
        if (userId != UserId)
            return Result.Fail(new ForbiddenError());
        if (CreatedAt.AddHours(1) <= DateTimeOffset.UtcNow)
            return Result.Fail(new RequestExceedTimeError(CreatedAt, nameof(PhoneAuthenticationRequest)));
        return Result.Ok();
    }
    public Result IsValidToConfirmPhoneAuthUsingIt(OtpManager _otpManager, string otpCode)
    {
        if (Otp != otpCode)
            return Result.Fail(new IncorrectOtpError(nameof(UpdatePhoneRequest)));

        if (!_otpManager.IsOtpValid(CreatedAt))
            return Result.Fail(new RequestExceedTimeError(CreatedAt, nameof(PhoneAuthenticationRequest)));
        return Result.Ok();
    }

}
