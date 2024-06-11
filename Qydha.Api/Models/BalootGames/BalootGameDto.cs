namespace Qydha.API.Models;

public class BalootGameDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public BalootGameMode GameMode { get; set; }
    public string State { get; set; } = null!;
    public string UsName { get; set; } = null!;
    public string ThemName { get; set; } = null!;
    public int UsGameScore { get; set; }
    public int ThemGameScore { get; set; }
    public BalootGameTeam? Winner { get; set; } = null;
    public int GameInterval { get; set; }
    public List<BalootSakkaDto> Sakkas { get; set; } = null!;

}

public class BalootSakkaDto
{
    public List<BalootMoshtaraDto> Moshtaras { get; set; } = null!;
    public bool IsMashdoda { get; set; }
    public BalootGameTeam? Winner { get; set; }
    public int UsSakkaScore { get; set; }
    public int ThemSakkaScore { get; set; }

}


public class BalootMoshtaraDto
{
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }
}
public class BalootGamesPage(List<BalootGameDto> items, int count, int pageNumber, int pageSize, int totalWins)
    : Page<BalootGameDto>(items, count, pageNumber, pageSize)
{
    public int TotalWins { get; set; } = totalWins;
}