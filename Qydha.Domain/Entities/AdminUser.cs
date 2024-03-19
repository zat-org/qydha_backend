
namespace Qydha.Domain.Entities;

public class AdminUser {
 
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string NormalizedUsername { get; set; } = null!;

    public AdminType Role { get; set; }

    public IEnumerable<Claim> GetClaims()
    {
        return
        [
            new ("sub", Id.ToString()),
            new ("userId", Id.ToString()),
            new ("username", Username ),
            new ("role", Role.ToString()),   // SuperAdmin , StaffAdmin
        ];
    }
}
