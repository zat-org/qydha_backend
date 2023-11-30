namespace Qydha.Domain.Entities;

public class RegistrationOTPRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password_Hash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public DateTime Created_On { get; set; } = DateTime.UtcNow;
    public Guid? User_Id { get; set; }
    public string? FCM_Token { get; set; }

    public RegistrationOTPRequest()
    {
        
    }
    public RegistrationOTPRequest(string username, string phone, string passwordHash, string otp, Guid? userId, string? fcmToken)
    {
        Username = username;
        Phone = phone;
        Password_Hash = passwordHash;
        OTP = otp;
        User_Id = userId is not null ? userId : null;
        FCM_Token = fcmToken;
    }
}
