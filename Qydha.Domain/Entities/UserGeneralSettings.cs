namespace Qydha.Domain.Entities;

[Table("user_general_settings")]
public class UserGeneralSettings
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("enable_vibration")]
    public bool EnableVibration { get; set; } = true;
    [JsonField]
    [Column("players_names")]
    public Json<IEnumerable<string>> PlayersNames { get; set; } = new List<string>();
    [JsonField]
    [Column("teams_names")]
    public Json<IEnumerable<string>> TeamsNames { get; set; } = new List<string>();
}
