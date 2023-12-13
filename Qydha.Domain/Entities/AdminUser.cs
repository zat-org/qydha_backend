
namespace Qydha.Domain.Entities;

[Table("Admins")]
public class AdminUser
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("password_hash")]
    public string Password_Hash { get; set; } = null!;
    [Column("created_at")]
    public DateTime Created_At { get; set; }

    [Column("normalized_username")]
    public string Normalized_Username { get { return Username.ToUpper(); } }

    [Column("role")]
    public AdminType Role { get; set; }

    public IEnumerable<Claim> GetClaims()
    {
        return new List<Claim>()
        {
            new ("sub", Id.ToString()),
            new ("userId", Id.ToString()),
            new ("username", Username ),
            new ("role", Role.ToString()),
        };
    }
}
