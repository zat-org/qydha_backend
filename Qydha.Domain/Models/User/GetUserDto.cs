namespace Qydha.Domain.Models;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? Name { get; set; }
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? ExpireDate { get; set; }
    public List<UserRoles> Roles { get; set; } = null!;
}
