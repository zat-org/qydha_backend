namespace Qydha.Domain.Entities;
public class RegistrationOTPRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string OTP { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? FCMToken { get; set; }
    public DateTimeOffset? UsedAt { get; set; } = null;
    public string SentBy { get; set; } = null!;

    public RegistrationOTPRequest() { }
    public RegistrationOTPRequest(string username, string phone, string passwordHash, string otp, string? fcmToken, string sender)
    {
        Username = username;
        Phone = phone;
        PasswordHash = passwordHash;
        OTP = otp;
        FCMToken = fcmToken;
        SentBy = sender;
    }
    public Result IsRequestValidToUse(OtpManager _otpManager, string otpCode)
    {
        if (OTP != otpCode)
            return Result.Fail(new IncorrectOtpError(nameof(RegistrationOTPRequest)));

        if (!_otpManager.IsOtpValid(CreatedAt))
            return Result.Fail(new RequestExceedTimeError(CreatedAt, nameof(RegistrationOTPRequest)));

        return Result.Ok();
    }
}
