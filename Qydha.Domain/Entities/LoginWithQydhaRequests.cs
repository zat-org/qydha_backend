namespace Qydha.Domain.Entities;

public class LoginWithQydhaRequest
{

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Otp { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public virtual User User { get; set; } = null!;

    public LoginWithQydhaRequest() { }
    public LoginWithQydhaRequest(Guid userId, string otp)
    {
        UserId = userId;
        Otp = otp;
        // ! utc
        CreatedAt = DateTime.Now;
    }
}
