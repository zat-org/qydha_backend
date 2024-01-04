
namespace Qydha.Domain.Entities;

[Table("Admins")]
[NotFoundError(ErrorType.AdminUserNotFound)]

public class AdminUser : DbEntity<AdminUser>
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("normalized_username")]
    public string NormalizedUsername { get; set; } = null!;

    [Column("role")]
    public AdminType Role { get; set; }

    public IEnumerable<Claim> GetClaims()
    {
        return new List<Claim>()
        {
            new ("sub", Id.ToString()),
            new ("userId", Id.ToString()),
            new ("username", Username ),
            new ("role", Role.ToString()),   // SuperAdmin , StaffAdmin
        };
    }
}
