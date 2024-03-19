namespace Qydha.Domain.Entities;
public class UserHandSettings
{
    public Guid UserId { get; set; }

    public int RoundsCount { get; set; } = 7;

    public int MaxLimit { get; set; } = 0;

    public int TeamsCount { get; set; } = 2;

    public int PlayersCountInTeam { get; set; } = 2;

    public bool WinUsingZat { get; set; } = false;

    public int TakweeshPoints { get; set; } = 100;

    public User User { get; set; } = null!;
}