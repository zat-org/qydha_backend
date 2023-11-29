namespace Qydha.Domain.Entities;

public class UpdatePhoneRequest
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public DateTime Created_On { get; set; } = DateTime.UtcNow;
    public Guid User_Id { get; set; }


    public UpdatePhoneRequest(string phone, string otp, Guid user_id)
    {
        Phone = phone;
        OTP = otp;
        User_Id = user_id;
        Created_On = DateTime.UtcNow;
    }
}
