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


    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.State)], [nameof(BalootGameDto.State)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.UsName)], [nameof(BalootGameDto.UsName)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.ThemName)], [nameof(BalootGameDto.ThemName)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.UsGameScore)], [nameof(BalootGameDto.UsGameScore)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.ThemGameScore)], [nameof(BalootGameDto.ThemGameScore)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.Winner)], [nameof(BalootGameDto.Winner)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.GameInterval)], [nameof(BalootGameDto.GameInterval)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.StartedAt)], [nameof(BalootGameDto.StartedAt)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.EndedAt)], [nameof(BalootGameDto.EndedAt)])]
    [MapProperty([nameof(BalootGame.State), nameof(BalootGame.State.Sakkas)], [nameof(BalootGameDto.Sakkas)])]
    [MapperIgnoreSource(nameof(BalootGame.EventsJsonString))]
    [MapperIgnoreSource(nameof(BalootGame.ModeratorId))]
    [MapperIgnoreSource(nameof(BalootGame.Moderator))]
    [MapperIgnoreSource(nameof(BalootGame.Owner))]
    [MapperIgnoreSource(nameof(BalootGame.OwnerId))]
    public partial BalootGameDto BalootGameToBalootGameDto(BalootGame game);

    [MapProperty(nameof(BalootSakkaState.Winner), nameof(BalootSakkaDto.Winner))]
    [MapProperty(nameof(BalootSakkaState.IsMashdoda), nameof(BalootSakkaDto.IsMashdoda))]
    [MapProperty(nameof(BalootSakkaState.Moshtaras), nameof(BalootSakkaDto.Moshtaras))]
    [MapProperty(nameof(BalootSakkaState.UsScore), nameof(BalootSakkaDto.UsSakkaScore))]
    [MapProperty(nameof(BalootSakkaState.ThemScore), nameof(BalootSakkaDto.ThemSakkaScore))]
    [MapperIgnoreSource(nameof(BalootSakkaState.State))]
    [MapperIgnoreSource(nameof(BalootSakkaState.IsRunningWithMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakkaState.IsCreated))]
    [MapperIgnoreSource(nameof(BalootSakkaState.IsRunningWithoutMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakkaState.CurrentMoshtara))]
    [MapperIgnoreSource(nameof(BalootSakkaState.WinningScore))]
    [MapperIgnoreSource(nameof(BalootSakkaState.DrawHandler))]
    [MapperIgnoreSource(nameof(BalootSakkaState.StartedAt))]
    [MapperIgnoreSource(nameof(BalootSakkaState.EndedAt))]
    [MapperIgnoreSource(nameof(BalootSakkaState.PausingIntervals))]
    [MapperIgnoreSource(nameof(BalootSakkaState.SakkaInterval))]
    public partial BalootSakkaDto BalootSakkaStateToSakkaDto(BalootSakkaState sakka);


    [MapProperty(nameof(BalootMoshtaraState.UsScore), nameof(BalootMoshtaraDto.UsAbnat))]
    [MapProperty(nameof(BalootMoshtaraState.ThemScore), nameof(BalootMoshtaraDto.ThemAbnat))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.MoshtaraData))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.MoshtaraInterval))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.State))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.StartedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.EndedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtaraState.PausingIntervals))]
    public partial BalootMoshtaraDto BalootMoshtaraStateToMoshtaraDto(BalootMoshtaraState moshtara);

    // [UseMapper()]
    // private int TimeSpanToSeconds(TimeSpan t) => t.Seconds;
}
