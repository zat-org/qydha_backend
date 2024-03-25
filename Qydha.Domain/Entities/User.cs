namespace Qydha.Domain.Entities;

public class User
{

    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Name { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public DateTimeOffset? BirthDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLogin { get; set; }

    public bool IsAnonymous { get; set; }

    public bool IsPhoneConfirmed { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? AvatarUrl { get; set; }

    public string? AvatarPath { get; set; }

    public DateTimeOffset? ExpireDate { get; set; } = null;

    public int FreeSubscriptionUsed { get; set; } = 0;

    public string? FCMToken { get; set; }

    public string NormalizedUsername { get; set; } = null!;

    public string? NormalizedEmail { get; set; } = null!;

    public UserGeneralSettings UserGeneralSettings { get; set; } = null!;
    public UserBalootSettings UserBalootSettings { get; set; } = null!;
    public UserHandSettings UserHandSettings { get; set; } = null!;
    public RegistrationOTPRequest? RegistrationOTPRequest { get; set; }
    public virtual ICollection<UserPromoCode> UserPromoCodes { get; set; } = [];
    public virtual ICollection<Purchase> Purchases { get; set; } = [];
    public virtual ICollection<LoginWithQydhaRequest> LoginWithQydhaRequests { get; set; } = [];
    public virtual ICollection<NotificationUserLink> NotificationUserLinks { get; set; } = [];
    public virtual ICollection<UpdateEmailRequest> UpdateEmailRequests { get; set; } = [];
    public virtual ICollection<UpdatePhoneRequest> UpdatePhoneRequests { get; set; } = [];
    public virtual ICollection<PhoneAuthenticationRequest> PhoneAuthenticationRequests { get; set; } = [];

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
            Username = otpRequest.Username,
            NormalizedUsername = otpRequest.Username.ToUpper(),
            PasswordHash = otpRequest.PasswordHash,
            Phone = otpRequest.Phone,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLogin = DateTimeOffset.UtcNow,
            IsPhoneConfirmed = true,
            IsAnonymous = false,
            FCMToken = otpRequest.FCMToken
        };
    }

}
