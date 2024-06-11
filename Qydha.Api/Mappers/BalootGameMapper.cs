
namespace Qydha.API.Mappers;
[Mapper]
public partial class BalootGameMapper
{
    public BalootGamesPage PageToGameArchiveDto(PagedList<BalootGame> games, int totalWins)
    {
        return new BalootGamesPage(games.Select(MapBalootGameToBalootGameDto).ToList(),
            games.TotalCount,
            games.CurrentPage,
            games.PageSize,
            totalWins);
    }

    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.StateName)], [nameof(BalootGameDto.State)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.UsName)], [nameof(BalootGameDto.UsName)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.ThemName)], [nameof(BalootGameDto.ThemName)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.UsGameScore)], [nameof(BalootGameDto.UsGameScore)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.ThemGameScore)], [nameof(BalootGameDto.ThemGameScore)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.Winner)], [nameof(BalootGameDto.Winner)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.GameInterval)], [nameof(BalootGameDto.GameInterval)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.StartedAt)], [nameof(BalootGameDto.StartedAt)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.EndedAt)], [nameof(BalootGameDto.EndedAt)])]
    [MapProperty([nameof(BalootGame.GameData), nameof(BalootGame.GameData.Sakkas)], [nameof(BalootGameDto.Sakkas)])]
    [MapperIgnoreSource(nameof(BalootGame.EventsJsonString))]
    [MapperIgnoreSource(nameof(BalootGame.ModeratorId))]
    [MapperIgnoreSource(nameof(BalootGame.Moderator))]
    [MapperIgnoreSource(nameof(BalootGame.Owner))]
    [MapperIgnoreSource(nameof(BalootGame.OwnerId))]
    private partial BalootGameDto BalootGameToBalootGameDto(BalootGame game);

    // [UserMapping(Default = true)]
    public BalootGameDto MapBalootGameToBalootGameDto(BalootGame game)
    {
        // custom before map code...
        var dto = BalootGameToBalootGameDto(game);
        // custom after map code...
        if (game.GameData.CurrentSakka.Moshtaras.Count > 0)
        {
            var currentSakkaDto = BalootSakkaStateToSakkaDto(game.GameData.CurrentSakka);
            dto.Sakkas.Add(currentSakkaDto);
        }
        return dto;
    }


    [MapProperty(nameof(BalootSakka.Winner), nameof(BalootSakkaDto.Winner))]
    [MapProperty(nameof(BalootSakka.IsMashdoda), nameof(BalootSakkaDto.IsMashdoda))]
    [MapProperty(nameof(BalootSakka.Moshtaras), nameof(BalootSakkaDto.Moshtaras))]
    [MapProperty(nameof(BalootSakka.UsScore), nameof(BalootSakkaDto.UsSakkaScore))]
    [MapProperty(nameof(BalootSakka.ThemScore), nameof(BalootSakkaDto.ThemSakkaScore))]
    [MapperIgnoreSource(nameof(BalootSakka.StateName))]
    [MapperIgnoreSource(nameof(BalootSakka.IsRunningWithMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakka.IsCreated))]
    [MapperIgnoreSource(nameof(BalootSakka.IsRunningWithoutMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakka.CurrentMoshtara))]
    [MapperIgnoreSource(nameof(BalootSakka.WinningScore))]
    [MapperIgnoreSource(nameof(BalootSakka.DrawHandler))]
    [MapperIgnoreSource(nameof(BalootSakka.StartedAt))]
    [MapperIgnoreSource(nameof(BalootSakka.EndedAt))]
    [MapperIgnoreSource(nameof(BalootSakka.PausingIntervals))]
    [MapperIgnoreSource(nameof(BalootSakka.SakkaInterval))]
    public partial BalootSakkaDto BalootSakkaStateToSakkaDto(BalootSakka sakka);


    [MapProperty(nameof(BalootMoshtara.UsScore), nameof(BalootMoshtaraDto.UsAbnat))]
    [MapProperty(nameof(BalootMoshtara.ThemScore), nameof(BalootMoshtaraDto.ThemAbnat))]
    [MapperIgnoreSource(nameof(BalootMoshtara.MoshtaraInterval))]
    [MapperIgnoreSource(nameof(BalootMoshtara.StateName))]
    [MapperIgnoreSource(nameof(BalootMoshtara.StartedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtara.EndedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtara.PausingIntervals))]
    public partial BalootMoshtaraDto BalootMoshtaraStateToMoshtaraDto(BalootMoshtara moshtara);

    private int TimeSpanToSeconds(TimeSpan t) => t.Seconds;
}
