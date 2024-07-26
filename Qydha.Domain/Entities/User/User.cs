namespace Qydha.Domain.Entities;

public class User : IClaimable
{
    public const string TokenType = "User";
    private User() { }

    public User(Guid? id, string username, string passwordHash, string phone, List<UserRoles>? roles = null, DateTimeOffset? createdAt = null)
    {
        Id = id ?? Guid.Empty;
        Username = username;
        PasswordHash = passwordHash;
        Phone = phone;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        Roles = roles ?? [UserRoles.User];
    }
    public Guid Id { get; set; }

    private string _username = null!;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            NormalizedUsername = value.ToUpper();
        }
    }
    public string NormalizedUsername { get; private set; } = null!;

    public string? Name { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? _email = null!;
    public string? Email
    {
        get => _email; set
        {
            _email = value;
            NormalizedEmail = value?.ToUpper();
        }
    }
    public string? NormalizedEmail { get; private set; }

    public DateTime? BirthDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLogin { get; set; }

    public string? AvatarUrl { get; set; }

    public string? AvatarPath { get; set; }

    public DateTimeOffset? ExpireDate { get; set; }

    public string? FCMToken { get; set; }



    public List<UserRoles> Roles { get; set; } = null!;

    public bool IsAdmin => Roles.Any(r => r == UserRoles.SuperAdmin || r == UserRoles.StaffAdmin);
    public bool IsSuperAdmin => Roles.Any(r => r == UserRoles.SuperAdmin);
    public bool IsStaffAdmin => Roles.Any(r => r == UserRoles.StaffAdmin);
    public UserGeneralSettings UserGeneralSettings { get; set; } = null!;
    public UserBalootSettings UserBalootSettings { get; set; } = null!;
    public UserHandSettings UserHandSettings { get; set; } = null!;
    public virtual ICollection<UserPromoCode> UserPromoCodes { get; set; } = [];
    public virtual ICollection<Purchase> Purchases { get; set; } = [];
    public virtual ICollection<LoginWithQydhaRequest> LoginWithQydhaRequests { get; set; } = [];
    public virtual ICollection<NotificationUserLink> NotificationUserLinks { get; set; } = [];
    public virtual ICollection<UpdateEmailRequest> UpdateEmailRequests { get; set; } = [];
    public virtual ICollection<UpdatePhoneRequest> UpdatePhoneRequests { get; set; } = [];
    public virtual ICollection<PhoneAuthenticationRequest> PhoneAuthenticationRequests { get; set; } = [];
    public virtual ICollection<InfluencerCodeUserLink> InfluencerCodes { get; set; } = [];
    public virtual ICollection<BalootGame> ModeratedBalootGames { get; set; } = [];
    public virtual ICollection<BalootGame> CreatedBalootGames { get; set; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];


    public IEnumerable<Claim> GetClaims()
    {
        var claims = new List<Claim>(){
                new(ClaimTypes.NameIdentifier, Id.ToString()),
                new(ClaimTypes.Name, Username),
                new(ClaimTypes.MobilePhone, Phone),
                new("SubscriptionExpireDate", ExpireDate?.ToString() ?? ""), // TODO use db to get the expire date not from jwt ,
                new(ClaimsNamesConstants.TokenType, TokenType)
        };
        Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.ToString())));
        return claims;
    }
    public Result ChangeUserRoles(List<UserRoles> roles)
    {
        var err = new InvalidBodyInputError("invalid roles list.");
        if (!roles.Contains(UserRoles.User))
        {
            err.ValidationErrors.Add(nameof(roles), ["Roles Must Contain User Role"]);
            return Result.Fail(err);
        }
        if (IsSuperAdmin && !roles.Contains(UserRoles.SuperAdmin))
        {
            err.ValidationErrors.Add(nameof(roles), ["SuperAdmin Role Can't removed from this user"]);
            return Result.Fail(err);
        }
        if (!IsSuperAdmin && roles.Contains(UserRoles.SuperAdmin))
        {
            err.ValidationErrors.Add(nameof(roles), ["SuperAdmin Role Can't Added to users"]);
            return Result.Fail(err);
        }
        Roles = roles;
        return Result.Ok();
    }
    public static User CreateUserFromRegisterRequest(RegistrationOTPRequest otpRequest)
    {
        return new User(
                id: null,
                username: otpRequest.Username,
                passwordHash: otpRequest.PasswordHash,
                phone: otpRequest.Phone
            )
        {
            LastLogin = DateTimeOffset.UtcNow,
            FCMToken = otpRequest.FCMToken,
            UserBalootSettings = new(),
            UserHandSettings = new(),
            UserGeneralSettings = new()
        };
    }
}

public enum UserRoles
{
    SuperAdmin = 1,
    StaffAdmin,
    User,
    Streamer
}
