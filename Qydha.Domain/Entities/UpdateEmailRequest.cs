namespace Qydha.Domain.Entities;

public class UpdateEmailRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string OTP { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public DateTime? UsedAt { get; set; } = null;


    public UpdateEmailRequest()
    {

    }
    public UpdateEmailRequest(Guid id, string email, string otp, Guid userId)
    {
        Id = id;
        Email = email;
        OTP = otp;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}
