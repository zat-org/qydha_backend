
using Qydha.Domain.Constants;

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
            CreatedAt = DateTimeOffset.UtcNow
        };
        _dbCtx.BalootGames.Add(balootGame);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(balootGame);
    }
    public async Task<Result<BalootGame>> GetById(Guid gameId)
    {
        var game = await _dbCtx.BalootGames.FirstOrDefaultAsync((g) => g.Id == gameId);
        if (game == null) return Result.Fail(new EntityNotFoundError<Guid>(gameId, nameof(BalootGame), ErrorType.BalootGameNotFound));
        return Result.Ok(game);
    }

    public async Task<Result> AddEvents(BalootGame game, ICollection<BalootGameEvent> events)
    {
        string eventsJsonString = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings);
        string stateJsonString = JsonConvert.SerializeObject(game.State, BalootConstants.balootEventsSerializationSettings);
        string gameId = game.Id.ToString();
        int affectedRows = await _dbCtx.Database.ExecuteSqlAsync(@$"
            UPDATE baloot_games
            SET game_events = COALESCE(game_events, '[]')::jsonb || {eventsJsonString}::jsonb,
                game_state = {stateJsonString}::jsonb
            WHERE id = {gameId}::uuid");
        if (affectedRows != 1)
            Result.Fail(new EntityNotFoundError<Guid>(game.Id, nameof(BalootGame), ErrorType.BalootGameNotFound));
        return Result.Ok();
    }

}
