
namespace Qydha.Infrastructure.Repositories;

public class BalootGamesRepo(QydhaContext qydhaContext) : IBalootGamesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    public async Task<Result<BalootGame>> CreateSingleBalootGame(Guid ownerId)
    {
        BalootGame balootGame = new()
        {
            OwnerId = ownerId,
            ModeratorId = ownerId,
            GameMode = BalootGameMode.SinglePlayer,
            CreatedAt = DateTimeOffset.UtcNow,
            Events = []
        };
        _dbCtx.BalootGames.Add(balootGame);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(balootGame);
    }
    public async Task<Result<BalootGame>> GetById(Guid gameId)
    {
        var game = await _dbCtx.BalootGames.AsTracking().FirstOrDefaultAsync((g) => g.Id == gameId);
        if (game == null) return Result.Fail<BalootGame>(new()
        {
            Code = ErrorType.BalootGameNotFound,
            Message = "Baloot Game Not Found"
        });
        return Result.Ok(game);
    }

    public async Task<Result<BalootGame>> AddEvents(BalootGame game, ICollection<BalootGameEvent> events)
    {
        game.Events.AddRange(events);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(game);
    }

}
