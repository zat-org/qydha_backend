namespace Qydha.API.Mappers;
[Mapper]

public partial class BalootGameMapper
{
    public BalootGamesPage PageToGameArchiveDto(PagedList<BalootGame> games, int totalWins)
    {
        return new BalootGamesPage(games.Select(BalootGameToBalootGameDto).ToList(),
            games.TotalCount,
            games.CurrentPage,
            games.PageSize,
            totalWins);
    }



    public partial BalootGameDto BalootGameToBalootGameDto(BalootGame game);

    private int TimeSpanToSeconds(TimeSpan t) => t.Seconds;


}
