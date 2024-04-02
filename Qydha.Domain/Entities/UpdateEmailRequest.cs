namespace Qydha.Domain.Entities;

public class UpdateEmailRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string OTP { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid UserId { get; set; }
    public DateTimeOffset? UsedAt { get; set; } = null;
    public string SentBy { get; set; } = null!;



    public UpdateEmailRequest()
    {

    }
    public UpdateEmailRequest(Guid id, string email, string otp, Guid userId, string sender)
    {
        Id = id;
        Email = email;
        OTP = otp;
        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
        SentBy = sender;
    }
}
