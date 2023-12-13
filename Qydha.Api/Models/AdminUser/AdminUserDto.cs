namespace Qydha.API.Models;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = null!;

}
