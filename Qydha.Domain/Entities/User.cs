namespace Qydha.Domain.Entities;

public class User
{

    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Name { get; set; }

    public string? PasswordHash { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool IsAnonymous { get; set; }

    public bool IsPhoneConfirmed { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? AvatarUrl { get; set; }

    public string? AvatarPath { get; set; }

    public DateTime? ExpireDate { get; set; } = null;

    public int FreeSubscriptionUsed { get; set; } = 0;

    public string FCMToken { get; set; } = string.Empty;

    public string? NormalizedUsername { get; set; }

    public string? NormalizedEmail { get; set; }

    public UserGeneralSettings? UserGeneralSettings { get; set; }
    public UserBalootSettings? UserBalootSettings { get; set; }
    public UserHandSettings? UserHandSettings { get; set; }
    public virtual ICollection<UserPromoCode> UserPromoCodes { get; set; } = [];
    public virtual ICollection<Purchase> Purchases { get; set; } = [];

    public virtual ICollection<LoginWithQydhaRequest> LoginWithQydhaRequests { get; set; } = [];

    public virtual ICollection<NotificationUserLink> NotificationUserLinks { get; set; } = [];

    public IEnumerable<Claim> GetClaims()
    {
        return
            [
                new ("sub", Id.ToString()),
                new ("userId", Id.ToString()),
                new ("username", Username ?? "" ),
                new ("phone", Phone ?? ""),
                new ("isAnonymous", IsAnonymous.ToString()),
                new ("role", !IsAnonymous ? "RegularUser" : "AnonymousUser"),
            ];
    }
    public static User CreateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User()
        {
            //TODO! Convert To UTC AGAIN
            Username = otpRequest.Username,
            NormalizedUsername = otpRequest.Username.ToUpper(),
            PasswordHash = otpRequest.PasswordHash,
            Phone = otpRequest.Phone,
            CreatedAt = DateTime.Now,
            LastLogin = DateTime.Now,
            IsPhoneConfirmed = true,
            IsAnonymous = false,
            FCMToken = otpRequest.FCMToken ?? ""
        };
    }
    public User UpdateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        //TODO! Convert To UTC AGAIN
        return new User()
        {
            Id = Id,
            Username = otpRequest.Username,
            NormalizedUsername = otpRequest.Username.ToUpper(),
            PasswordHash = otpRequest.PasswordHash,
            Phone = otpRequest.Phone,
            CreatedAt = CreatedAt,
            LastLogin = DateTime.Now,
            IsPhoneConfirmed = true,
            IsAnonymous = false,
            FCMToken = !string.IsNullOrEmpty(otpRequest.FCMToken) ? otpRequest.FCMToken : FCMToken
        };
    }
}
