
namespace Qydha.Domain.Services.Implementation;

public class BalootGamesService(IBalootGamesRepo balootGamesRepo, IMediator mediator, ILogger<BalootGamesService> logger) : IBalootGamesService
{
    private readonly IBalootGamesRepo _balootGamesRepo = balootGamesRepo;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<BalootGamesService> _logger = logger;
    private async Task<Result<BalootGame>> ApplyEventsAndSaveTheGame(BalootGame game, ICollection<BalootGameEvent> events)
    {
        foreach (var e in events)
        {
            Result res = e.ApplyToState(game);
            if (res.IsFailed)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["events"] = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings)
                }))
                {
                    _logger.LogWarning("Baloot Game Error with message :: \" {msg} \" While trying to Create game with the events", res.Errors.First().Message);
                }

                return res;
            }
        }
        game.EventsJsonString = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings);
        return await _balootGamesRepo.SaveGame(game);
    }
    public async Task<Result<BalootGame>> CreateSingleBalootGame(Guid userId, ICollection<BalootGameEvent> events, DateTimeOffset createdAt, XInfoData xInfoData) =>
        (await ApplyEventsAndSaveTheGame(BalootGame.CreateSinglePlayerGame(userId, createdAt, xInfoData), events))
        .OnSuccessAsync(async (game) =>
        {
            if (game.OwnerId != null)
                await _mediator.Publish(new ApplyEventsToBalootGameNotification(game.OwnerId.Value, game));
            return Result.Ok(game);
        });

    public async Task<Result<BalootGame>> CreateAnonymousBalootGame(ICollection<BalootGameEvent> events, DateTimeOffset createdAt, XInfoData xInfoData) =>
            await ApplyEventsAndSaveTheGame(BalootGame.CreateAnonymousGame(createdAt, xInfoData), events);

    public async Task<Result<BalootGame>> AddEvents(Guid userId, Guid gameId, ICollection<BalootGameEvent> events, bool hasServiceAccountPermission = false)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccessAsync(async (game) =>
            {
                if ((game.GameMode == BalootGameMode.AnonymousGame && !hasServiceAccountPermission) ||
                    (game.GameMode != BalootGameMode.AnonymousGame && userId != game.ModeratorId && userId != game.OwnerId))
                    return Result.Fail(new ForbiddenError());

                foreach (var e in events)
                {
                    Result res = e.ApplyToState(game);
                    if (res.IsFailed)
                    {
                        using (_logger.BeginScope(new Dictionary<string, object>
                        {
                            ["events"] = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings)
                        }))
                        {
                            _logger.LogWarning("Baloot Game Error with message :: {msg} While trying to Apply these events userId : {userId} , gameId : {gameId} ", res.Errors.First().Message, userId, gameId);
                        }
                        return res;
                    }
                }
                return (await _balootGamesRepo.AddEvents(game, events)).ToResult(game);
            })
            .OnSuccessAsync(async (game) =>
            {
                if (game.GameMode != BalootGameMode.AnonymousGame && game.OwnerId != null)
                    await _mediator.Publish(new ApplyEventsToBalootGameNotification(game.OwnerId.Value, game));
                return Result.Ok(game);
            });
    }

    public async Task<Result<BalootGame>> GetGameById(Guid requesterId, Guid gameId, bool isRequesterAdmin = false)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccess((game) =>
            {
                if (isRequesterAdmin)
                    return Result.Ok(game);
                else
                {
                    if (requesterId != game.ModeratorId && requesterId != game.OwnerId)
                        return Result.Fail(new ForbiddenError());
                    return Result.Ok(game);
                }

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

    public async Task<Result> DeleteByIds(Guid gameId, Guid userId, bool hasServiceAccountPermission = false) =>
        await _balootGamesRepo.DeleteByIds(gameId, userId, hasServiceAccountPermission);
}
