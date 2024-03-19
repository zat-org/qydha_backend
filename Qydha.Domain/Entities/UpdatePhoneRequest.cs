namespace Qydha.Domain.Entities;
public class UpdatePhoneRequest
{

    public Guid Id { get; set; }

    public string Phone { get; set; } = null!;

    public string OTP { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Guid UserId { get; set; }

    public UpdatePhoneRequest()
    {

    }
    public UpdatePhoneRequest(string phone, string otp, Guid user_id)
    {
        Phone = phone;
        OTP = otp;
        UserId = user_id;
        //! TODO convert to utc
        CreatedAt = DateTime.Now;
    }
}
