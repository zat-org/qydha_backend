namespace Qydha.Domain.Entities;
[Table("users")]
[NotFoundError(ErrorType.UserNotFound)]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("birth_date")]
    public DateTime? BirthDate { get; set; }

    [Column("created_on")]
    public DateTime CreatedAt { get; set; }

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("is_anonymous")]
    public bool IsAnonymous { get; set; }

    [Column("is_phone_confirmed")]
    public bool IsPhoneConfirmed { get; set; }

    [Column("is_email_confirmed")]
    public bool IsEmailConfirmed { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("avatar_path")]
    public string? AvatarPath { get; set; }

    [Column("expire_date")]
    public DateTime? ExpireDate { get; set; } = null;

    [Column("free_subscription_used")]
    public int FreeSubscriptionUsed { get; set; } = 0;

    [Column("fcm_token")]
    public string FCMToken { get; set; } = string.Empty;

    [Column("normalized_username")]
    public string? NormalizedUsername { get; set; }

    [Column("normalized_email")]
    public string? NormalizedEmail { get; set; }


    public static User CreateAnonymousUser()
    {
        return new User()
        {
            CreatedAt = DateTime.UtcNow,
            IsAnonymous = true
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
                new ("isAnonymous", IsAnonymous.ToString()),
            };
    }
    public static User CreateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User()
        {
            Username = otpRequest.Username,
            NormalizedUsername = otpRequest.Username.ToUpper(),
            PasswordHash = otpRequest.PasswordHash,
            Phone = otpRequest.Phone,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            IsPhoneConfirmed = true,
            IsAnonymous = false,
            FCMToken = otpRequest.FCMToken ?? ""
        };
    }
    public User UpdateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User()
        {
            Id = Id,
            Username = otpRequest.Username,
            NormalizedUsername = otpRequest.Username.ToUpper(),
            PasswordHash = otpRequest.PasswordHash,
            Phone = otpRequest.Phone,
            CreatedAt = CreatedAt,
            LastLogin = DateTime.UtcNow,
            IsPhoneConfirmed = true,
            IsAnonymous = false,
            FCMToken = !string.IsNullOrEmpty(otpRequest.FCMToken) ? otpRequest.FCMToken : FCMToken
        };
    }
}
