namespace Qydha.API.Models;

public class BalootGameDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public BalootGameMode GameMode { get; set; }
    public BalootGameState State { get; set; } = null!;

}

public class BalootGamesPage : Page<BalootGameDto>
{
    public int TotalWins { get; set; }
    public BalootGamesPage(List<BalootGameDto> items, int count, int pageNumber, int pageSize, int totalWins)
        : base(items, count, pageNumber, pageSize)
    {
        TotalWins = totalWins;
    }

}