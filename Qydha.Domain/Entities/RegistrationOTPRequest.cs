namespace Qydha.Domain.Entities;
public class RegistrationOTPRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string OTP { get; set; } = null!;
    // ! todo convert to utc
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Guid? UserId { get; set; }
    public string? FCMToken { get; set; }

    public RegistrationOTPRequest() { }
    public RegistrationOTPRequest(string username, string phone, string passwordHash, string otp, Guid? userId, string? fcmToken)
    {
        Username = username;
        Phone = phone;
        PasswordHash = passwordHash;
        OTP = otp;
        UserId = userId is not null ? userId : null;
        FCMToken = fcmToken;
    }
}
