namespace Qydha.Domain.Entities;

public class PhoneAuthenticationRequest
{
    public Guid Id { get; set; }
    public string Otp { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime? UsedAt { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public PhoneAuthenticationRequest() { }
    public PhoneAuthenticationRequest(Guid userId, string otp)
    {
        UserId = userId;
        Otp = otp;
        CreatedAt = DateTime.UtcNow;
    }
}
