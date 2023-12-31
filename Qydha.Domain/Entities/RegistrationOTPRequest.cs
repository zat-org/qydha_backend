namespace Qydha.Domain.Entities;

[Table("registration_otp_request")]
[NotFoundError(ErrorType.RegistrationOTPRequestNotFound)]

public class RegistrationOTPRequest : DbEntity<RegistrationOTPRequest>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("username")]
    public string Username { get; set; } = string.Empty;
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;
    [Column("phone")]
    public string Phone { get; set; } = string.Empty;
    [Column("otp")]
    public string OTP { get; set; } = string.Empty;
    [Column("created_on")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("user_id")]
    public Guid? UserId { get; set; }
    [Column("fcm_token")]
    public string? FCMToken { get; set; }

    public RegistrationOTPRequest()
    {

    }
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
