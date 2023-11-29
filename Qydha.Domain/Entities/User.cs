
namespace Qydha.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? Password_Hash { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? Birth_Date { get; set; }
    public DateTime Created_On { get; set; }
    public DateTime? Last_Login { get; set; }
    public bool Is_Anonymous { get; set; }
    public bool Is_Phone_Confirmed { get; set; }
    public bool Is_Email_Confirmed { get; set; }
    public string? Avatar_Url { get; set; }
    public string? Avatar_Path { get; set; }
    public DateTime? Expire_Date { get; set; } = null;
    public int Free_Subscription_Used { get; set; } = 0;
    public string FCM_Token { get; set; } = string.Empty;


    public static User CreateAnonymousUser()
    {
        return new User()
        {
            Created_On = DateTime.UtcNow,
            Is_Anonymous = true
        };
    }
    public IEnumerable<Claim> GetClaims()
    {
        return new List<Claim>()
            {
                new ("sub", Id.ToString()),
                new ("userId", Id.ToString()),
                new ("username", Username ?? "" ),
                new ("phone", Phone ?? ""),
                new ("isAnonymous", Is_Anonymous.ToString()),
            };
    }
    public static User CreateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User()
        {
            Username = otpRequest.Username,
            Password_Hash = otpRequest.Password_Hash,
            Phone = otpRequest.Phone,
            Created_On = DateTime.UtcNow,
            Last_Login = DateTime.UtcNow,
            Is_Phone_Confirmed = true,
            Is_Anonymous = false,
            FCM_Token = otpRequest.FCM_Token ?? ""
        };
    }
    public User UpdateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User()
        {
            Id = Id,
            Username = otpRequest.Username,
            Password_Hash = otpRequest.Password_Hash,
            Phone = otpRequest.Phone,
            Created_On = Created_On,
            Last_Login = DateTime.UtcNow,
            Is_Phone_Confirmed = true,
            Is_Anonymous = false,
            FCM_Token = !string.IsNullOrEmpty(otpRequest.FCM_Token) ? otpRequest.FCM_Token : FCM_Token
        };
    }
}
