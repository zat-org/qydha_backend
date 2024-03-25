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
}
