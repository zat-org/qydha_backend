namespace Qydha.Domain.Entities;

public class UserGeneralSettings
{
    public Guid UserId { get; set; }
    public bool? EnableVibration { get; set; } = true;
    public IEnumerable<string> PlayersNames { get; set; } = [];
    public IEnumerable<string> TeamsNames { get; set; } = [];
    public User User { get; set; } = null!;
}
