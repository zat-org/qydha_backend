
using NetTopologySuite.Geometries;

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

    [MapProperty([nameof(BalootGame.Id)], [nameof(BalootGameDto.Id)])]
    [MapProperty([nameof(BalootGame.CreatedAt)], [nameof(BalootGameDto.CreatedAt)])]
    [MapProperty([nameof(BalootGame.StartedAt)], [nameof(BalootGameDto.StartedAt)])]
    [MapProperty([nameof(BalootGame.EndedAt)], [nameof(BalootGameDto.EndedAt)])]
    [MapProperty([nameof(BalootGame.GameMode)], [nameof(BalootGameDto.GameMode)])]
    [MapProperty([nameof(BalootGame.StateName)], [nameof(BalootGameDto.State)])]
    [MapProperty([nameof(BalootGame.UsName)], [nameof(BalootGameDto.UsName)])]
    [MapProperty([nameof(BalootGame.ThemName)], [nameof(BalootGameDto.ThemName)])]
    [MapProperty([nameof(BalootGame.MaxSakkaPerGame)], [nameof(BalootGameDto.MaxSakkaPerGame)])]
    [MapProperty([nameof(BalootGame.UsGameScore)], [nameof(BalootGameDto.UsGameScore)])]
    [MapProperty([nameof(BalootGame.ThemGameScore)], [nameof(BalootGameDto.ThemGameScore)])]
    [MapProperty([nameof(BalootGame.Winner)], [nameof(BalootGameDto.Winner)])]
    [MapProperty([nameof(BalootGame.GameInterval)], [nameof(BalootGameDto.GameInterval)])]
    [MapProperty([nameof(BalootGame.Location)], [nameof(BalootGameDto.Location)])]
    [MapProperty([nameof(BalootGame.Sakkas)], [nameof(BalootGameDto.Sakkas)])]
    [MapperIgnoreSource(nameof(BalootGame.EventsJsonString))]
    [MapperIgnoreSource(nameof(BalootGame.ModeratorId))]
    [MapperIgnoreSource(nameof(BalootGame.Moderator))]
    [MapperIgnoreSource(nameof(BalootGame.Owner))]
    [MapperIgnoreSource(nameof(BalootGame.OwnerId))]
    [MapperIgnoreSource(nameof(BalootGame.IsEnded))]
    [MapperIgnoreSource(nameof(BalootGame.IsRunningWithoutSakkas))]
    [MapperIgnoreSource(nameof(BalootGame.IsRunningWithSakkas))]
    [MapperIgnoreSource(nameof(BalootGame.IsCreated))]
    [MapperIgnoreSource(nameof(BalootGame.PausingIntervals))]
    [MapperIgnoreSource(nameof(BalootGame.CurrentSakka))]

    public partial BalootGameDto BalootGameToBalootGameDto(BalootGame game);

    // [UserMapping(Default = true)]
    public BalootGameDto MapBalootGameToBalootGameDto(BalootGame game)
    {
        // custom before map code...
        return BalootGameToBalootGameDto(game);
        // custom after map code...
        // if (game.CurrentSakka.Moshtaras.Count > 0)
        // {
        //     var currentSakkaDto = BalootSakkaStateToSakkaDto(game.CurrentSakka);
        //     dto.Sakkas.Add(currentSakkaDto);
        // }
        // return dto;
    }

    [MapProperty([nameof(BalootSakka.Id)], [nameof(BalootSakkaDto.Id)])]
    [MapProperty(nameof(BalootSakka.Winner), nameof(BalootSakkaDto.Winner))]
    [MapProperty(nameof(BalootSakka.IsMashdoda), nameof(BalootSakkaDto.IsMashdoda))]
    [MapProperty(nameof(BalootSakka.Moshtaras), nameof(BalootSakkaDto.Moshtaras))]
    [MapProperty(nameof(BalootSakka.UsScore), nameof(BalootSakkaDto.UsSakkaScore))]
    [MapProperty(nameof(BalootSakka.ThemScore), nameof(BalootSakkaDto.ThemSakkaScore))]
    [MapProperty(nameof(BalootSakka.StateName), nameof(BalootSakkaDto.State))]
    [MapperIgnoreSource(nameof(BalootSakka.IsRunningWithMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakka.IsCreated))]
    [MapperIgnoreSource(nameof(BalootSakka.IsEnded))]
    [MapperIgnoreSource(nameof(BalootSakka.BalootGameId))]
    [MapperIgnoreSource(nameof(BalootSakka.IsRunningWithoutMoshtaras))]
    [MapperIgnoreSource(nameof(BalootSakka.CurrentMoshtara))]
    [MapperIgnoreSource(nameof(BalootSakka.WinningScore))]
    [MapperIgnoreSource(nameof(BalootSakka.DrawHandler))]
    [MapperIgnoreSource(nameof(BalootSakka.StartedAt))]
    [MapperIgnoreSource(nameof(BalootSakka.EndedAt))]
    [MapperIgnoreSource(nameof(BalootSakka.PausingIntervals))]
    [MapperIgnoreSource(nameof(BalootSakka.SakkaInterval))]
    public partial BalootSakkaDto BalootSakkaStateToSakkaDto(BalootSakka sakka);

    [MapProperty([nameof(BalootMoshtara.Id)], [nameof(BalootMoshtaraDto.Id)])]
    [MapProperty(nameof(BalootMoshtara.UsScore), nameof(BalootMoshtaraDto.UsAbnat))]
    [MapProperty(nameof(BalootMoshtara.ThemScore), nameof(BalootMoshtaraDto.ThemAbnat))]
    [MapProperty(nameof(BalootMoshtara.StateName), nameof(BalootMoshtaraDto.State))]
    [MapperIgnoreSource(nameof(BalootMoshtara.MoshtaraInterval))]
    [MapperIgnoreSource(nameof(BalootMoshtara.StartedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtara.EndedAt))]
    [MapperIgnoreSource(nameof(BalootMoshtara.PausingIntervals))]
    [MapperIgnoreSource(nameof(BalootMoshtara.IsCreated))]
    [MapperIgnoreSource(nameof(BalootMoshtara.IsRunning))]
    [MapperIgnoreSource(nameof(BalootMoshtara.IsEnded))]
    [MapperIgnoreSource(nameof(BalootMoshtara.IsPaused))]
    [MapperIgnoreSource(nameof(BalootMoshtara.Data))]
    [MapperIgnoreSource(nameof(BalootMoshtara.BalootSakkaId))]
    public partial BalootMoshtaraDto BalootMoshtaraStateToMoshtaraDto(BalootMoshtara moshtara);

    private static int TimeSpanToSeconds(TimeSpan t) => t.Seconds;
    private static Location PointToLocation(Point p) => new(p.X, p.Y);
}
public record Location(double Longitude, double Latitude);
