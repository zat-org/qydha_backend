﻿namespace Qydha.Domain.Entities;
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
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    public RegistrationOTPRequest() { }
    public RegistrationOTPRequest(string username, string phone, string passwordHash, string otp, string? fcmToken)
    {
        Username = username;
        Phone = phone;
        PasswordHash = passwordHash;
        OTP = otp;
        FCMToken = fcmToken;
    }
}
