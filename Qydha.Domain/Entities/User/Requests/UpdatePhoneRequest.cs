namespace Qydha.Domain.Entities;
public class UpdatePhoneRequest
{

    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string OTP { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid UserId { get; set; }
    public DateTimeOffset? UsedAt { get; set; } = null;
    public string SentBy { get; set; } = null!;


    public UpdatePhoneRequest()
    {

    }
    public UpdatePhoneRequest(string phone, string otp, Guid user_id, string sender)
    {
        Phone = phone;
        OTP = otp;
        UserId = user_id;
        CreatedAt = DateTimeOffset.UtcNow;
        SentBy = sender;
    }
    public Result IsRequestValidToUse(Guid userId, string code, OtpManager _otpManager)
    {
        if (!_otpManager.IsOtpValid(CreatedAt))
            return Result.Fail(new RequestExceedTimeError(CreatedAt, nameof(UpdatePhoneRequest)));

        if (OTP != code || UserId != userId)
            return Result.Fail(new IncorrectOtpError(nameof(UpdatePhoneRequest)));

        return Result.Ok();
    }
}
