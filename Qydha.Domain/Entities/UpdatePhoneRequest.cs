namespace Qydha.Domain.Entities;

[Table("update_phone_requests")]
[NotFoundError(ErrorType.UpdatePhoneRequestNotFound)]

public class UpdatePhoneRequest : DbEntity<UpdatePhoneRequest>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("otp")]
    public string OTP { get; set; } = string.Empty;

    [Column("created_on")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("user_id")]
    public Guid UserId { get; set; }

    public UpdatePhoneRequest()
    {

    }
    public UpdatePhoneRequest(string phone, string otp, Guid user_id)
    {
        Phone = phone;
        OTP = otp;
        UserId = user_id;
        CreatedAt = DateTime.UtcNow;
    }
}
