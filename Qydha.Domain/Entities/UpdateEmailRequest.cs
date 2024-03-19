namespace Qydha.Domain.Entities;

public class UpdateEmailRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string OTP { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Guid UserId { get; set; }

    public UpdateEmailRequest()
    {

    }
    public UpdateEmailRequest(Guid id, string email, string otp, Guid userId)
    {
        Id = id;
        Email = email;
        OTP = otp;
        UserId = userId;
        // ! Todo Convert to utc 
        CreatedAt = DateTime.Now;
    }
}
