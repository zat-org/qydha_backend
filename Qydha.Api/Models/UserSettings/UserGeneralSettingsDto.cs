namespace Qydha.API.Models;

public class UserGeneralSettingsDto
{
    public bool EnableVibration { get; set; }
    public IEnumerable<string> PlayersNames { get; set; } = null!;
    public IEnumerable<string> TeamsNames { get; set; } = null!;

}
