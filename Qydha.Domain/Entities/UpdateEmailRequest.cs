namespace Qydha.Domain.Entities;

[Table("update_email_requests")]
[NotFoundError(ErrorType.UpdateEmailRequestNotFound)]
public class UpdateEmailRequest
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("otp")]
    public string OTP { get; set; } = string.Empty;
    [Column("created_on")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("user_id")]
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
        CreatedAt = DateTime.UtcNow;
    }
}
