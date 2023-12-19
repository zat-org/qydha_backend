namespace Qydha.Domain.Entities;

[Table("phone_authentication_requests")]
[NotFoundError(ErrorType.PhoneAuthenticationRequestNotFound)]

public class PhoneAuthenticationRequest
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("phone")]
    public string Phone { get; set; } = null!;
    [Column("otp")]
    public string Otp { get; set; } = null!;
    [Column("created_on")]
    public DateTime CreatedAt { get; set; }
    public PhoneAuthenticationRequest()
    {

    }
    public PhoneAuthenticationRequest(string phone, string otp)
    {
        Phone = phone;
        Otp = otp;
        CreatedAt = DateTime.UtcNow;
    }
}
