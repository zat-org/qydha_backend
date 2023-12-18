namespace Qydha.Domain.Entities;

[Table("user_general_settings")]
public class UserGeneralSettings
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("enable_vibration")]
    public bool EnableVibration { get; set; }
    [Column("enable_notifications")]
    public bool EnableNotifications { get; set; }
    [Column("players_names")]
    public IEnumerable<string> PlayersNames { get; set; } = new List<string>();
    [Column("teams_names")]
    public IEnumerable<string> TeamsNames { get; set; } = new List<string>();
}
