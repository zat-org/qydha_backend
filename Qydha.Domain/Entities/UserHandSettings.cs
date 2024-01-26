
namespace Qydha.Domain.Entities;

[Table("user_hand_settings")]
public class UserHandSettings : DbEntity<UserHandSettings>
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("rounds_count")]
    public int RoundsCount { get; set; } = 7;

    [Column("max_limit")]
    public int MaxLimit { get; set; } = 0;

    [Column("teams_count")]
    public int TeamsCount { get; set; } = 2;

    [Column("players_count_in_team")]
    public int PlayersCountInTeam { get; set; } = 2;

    [Column("win_using_zat")]
    public bool WinUsingZat { get; set; } = false;

    [Column("takweesh_points")]
    public int TakweeshPoints { get; set; } = 100;
}