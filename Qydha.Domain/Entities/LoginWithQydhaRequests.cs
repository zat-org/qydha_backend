namespace Qydha.Domain.Entities;

[Table("Login_with_qydha_Requests")]
[NotFoundError(ErrorType.LoginWithQydhaRequestNotFound)]

public class LoginWithQydhaRequest : DbEntity<LoginWithQydhaRequest>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("otp")]
    public string Otp { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }
    public LoginWithQydhaRequest()
    {

    }
    public LoginWithQydhaRequest(Guid userId, string otp)
    {
        UserId = userId;
        Otp = otp;
        CreatedAt = DateTime.UtcNow;
    }
}
