namespace Qydha.Domain.Entities;

public class PhoneAuthenticationRequest
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string Otp { get; set; } = null!;
    //! TODO to utc 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public PhoneAuthenticationRequest() { }
    public PhoneAuthenticationRequest(string phone, string otp)
    {
        Phone = phone;
        Otp = otp;
        //! TODO to utc 
        CreatedAt = DateTime.Now;
    }
}
