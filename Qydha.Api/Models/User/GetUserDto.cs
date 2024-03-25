namespace Qydha.API.Models;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public bool IsAnonymous { get; set; }
    public bool IsPhoneConfirmed { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? ExpireDate { get; set; }
    public int FreeSubscriptionUsed { get; set; }
}

