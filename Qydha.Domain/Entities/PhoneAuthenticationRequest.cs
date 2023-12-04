namespace Qydha.Domain.Entities;

public class PhoneAuthenticationRequest
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string Otp { get; set; } = null!;
    public DateTime Created_On { get; set; }
    public PhoneAuthenticationRequest()
    {

    }
    public PhoneAuthenticationRequest(string phone, string otp)
    {
        Phone = phone;
        Otp = otp;
        Created_On = DateTime.UtcNow;
    }
}
