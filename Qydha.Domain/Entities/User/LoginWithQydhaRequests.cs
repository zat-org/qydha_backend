namespace Qydha.Domain.Entities;

public class LoginWithQydhaRequest
{

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Otp { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public virtual User User { get; set; } = null!;

    public LoginWithQydhaRequest() { }
    public LoginWithQydhaRequest(Guid userId, string otp)
    {
        UserId = userId;
        Otp = otp;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    public Result IsRequestValidToUse(OtpManager _otpManager, string otpCode)
    {
        if (UsedAt != null)
            return Result.Fail(new OtpAlreadyUsedError(UsedAt.Value, nameof(LoginWithQydhaRequest)));
        if (Otp != otpCode)
            return Result.Fail(new IncorrectOtpError(nameof(LoginWithQydhaRequest)));
        if (!_otpManager.IsOtpValid(CreatedAt))
            return Result.Fail(new RequestExceedTimeError(CreatedAt, nameof(LoginWithQydhaRequest), ErrorType.OTPExceededTimeLimit));
        return Result.Ok();
    }
}
