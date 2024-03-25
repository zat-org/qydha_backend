namespace Qydha.Domain.Entities;

public class PhoneAuthenticationRequest
{
    public Guid Id { get; set; }
    public string Otp { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTimeOffset? UsedAt { get; set; } = null;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public PhoneAuthenticationRequest() { }
    public PhoneAuthenticationRequest(Guid userId, string otp)
    {
        UserId = userId;
        Otp = otp;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
