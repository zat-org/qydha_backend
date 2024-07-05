
namespace Qydha.Domain.Services.Implementation;

public class BalootGamesService(IBalootGamesRepo balootGamesRepo) : IBalootGamesService
{
    private readonly IBalootGamesRepo _balootGamesRepo = balootGamesRepo;
    public async Task<Result<BalootGame>> CreateSingleBalootGame(Guid userId, ICollection<BalootGameEvent> events)
    {
        BalootGame balootGame = BalootGame.CreateSinglePlayerGame(userId);
        foreach (var e in events)
        {
            Result res = e.ApplyToState(balootGame);
            if (res.IsFailed) return res;
        }
        balootGame.EventsJsonString = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings);
        return await _balootGamesRepo.SaveGame(balootGame);
    }

    public async Task<Result<BalootGame>> AddEvents(Guid userId, Guid gameId, ICollection<BalootGameEvent> events)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccessAsync(async (game) =>
            {
                if (userId != game.ModeratorId && userId != game.OwnerId)
                    return Result.Fail(new ForbiddenError());

                foreach (var e in events)
                {
                    Result res = e.ApplyToState(game);
                    if (res.IsFailed) return res;
                }
                return (await _balootGamesRepo.AddEvents(game, events)).ToResult(game);
            });
    }

    public async Task<Result<BalootGame>> GetGameById(Guid requesterId, Guid gameId)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccess((game) =>
            {
                if (requesterId != game.ModeratorId && requesterId != game.OwnerId)
                    return Result.Fail(new ForbiddenError());
                return Result.Ok(game);
            });
    }

    public async Task<Result<(PagedList<BalootGame> Games, int WinsCount)>> GetUserArchive(Guid userId, PaginationParameters parameters)
    {
        return (await _balootGamesRepo.GetUserBalootGamesArchive(userId, parameters))
        .OnSuccessAsync(async (games) => (await _balootGamesRepo.GetUserBalootGamesWinsCount(userId))
            .ToResult((winsCount) => (games, winsCount)));
    }



    public async Task<Result<List<BalootGameTimeLineBlock>>> GetGameTimeLineById(Guid gameId)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccess((game) => Result.Ok(game.GetGameTimelineForEditing()));
    }

    public async Task<Result<BalootGameStatistics>> GetGameStatisticsById(Guid gameId)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccess((game) => Result.Ok(game.GetStatistics()));
    }

    public async Task<Result> DeleteById(Guid gameId, Guid userId) =>
        await _balootGamesRepo.DeleteById(gameId, userId);
}
