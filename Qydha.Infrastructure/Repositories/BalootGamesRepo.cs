
using Qydha.Domain.Constants;

namespace Qydha.Infrastructure.Repositories;

public class BalootGamesRepo(QydhaContext qydhaContext) : IBalootGamesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    public async Task<Result<BalootGame>> SaveGame(BalootGame game)
    {
        if (game.OwnerId != null && !_dbCtx.Users.Any(u => u.Id == game.OwnerId))
            return Result.Fail(new EntityNotFoundError<Guid>(game.OwnerId.Value, nameof(User)));
        if (game.ModeratorId != null && !_dbCtx.Users.Any(u => u.Id == game.ModeratorId))
            return Result.Fail(new EntityNotFoundError<Guid>(game.ModeratorId.Value, nameof(User)));
        _dbCtx.BalootGames.Add(game);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(game);
    }
    public async Task<Result<BalootGame>> GetById(Guid gameId)
    {
        var game = await _dbCtx.BalootGames
            .AsTracking(QueryTrackingBehavior.TrackAll)
            .AsSplitQuery()
            .Include(g => g.Sakkas.OrderBy(s => s.Id))
            .ThenInclude(s => s.Moshtaras.OrderBy(m => m.Id))
            .FirstOrDefaultAsync((g) => g.Id == gameId);
        if (game == null) return Result.Fail(new EntityNotFoundError<Guid>(gameId, nameof(BalootGame)));
        return Result.Ok(game);
    }

    public async Task<Result> AddEvents(BalootGame game, ICollection<BalootGameEvent> events)
    {
        string eventsJsonString = JsonConvert.SerializeObject(events, BalootConstants.balootEventsSerializationSettings);
        string gameId = game.Id.ToString();
        int affectedRows = await _dbCtx.Database.ExecuteSqlAsync(@$"
            UPDATE baloot_games
            SET game_events = COALESCE(game_events, '[]')::jsonb || {eventsJsonString}::jsonb
            WHERE id = {gameId}::uuid");
        await _dbCtx.SaveChangesAsync();
        if (affectedRows != 1)
            Result.Fail(new EntityNotFoundError<Guid>(game.Id, nameof(BalootGame)));
        return Result.Ok();
    }

    public async Task<Result<PagedList<BalootGame>>> GetUserBalootGamesArchive(Guid userId, PaginationParameters parameters)
    {
        var query = _dbCtx.BalootGames
            .Where(g => g.OwnerId == userId)
            .Include(g => g.Sakkas.OrderBy(s => s.Id))
            .ThenInclude(s => s.Moshtaras.OrderBy(m => m.Id))
            .OrderByDescending(g => g.CreatedAt);
        PagedList<BalootGame> games = await _dbCtx.GetPagedData(query, parameters.PageNumber, parameters.PageSize);
        return Result.Ok(games);
    }
    public async Task<Result<int>> GetUserBalootGamesWinsCount(Guid userId)
    {
        int winsCount = await _dbCtx.BalootGames
            .Where(g => g.OwnerId == userId && g.StateName == BalootGameStateEnum.Ended && g.Winner == BalootGameTeam.Us)
            .CountAsync();
        return Result.Ok(winsCount);
    }
    public async Task<Result> DeleteByIds(Guid gameId, Guid userId, bool hasServiceAccountPermission = false)
    {
        var affected = await _dbCtx
            .BalootGames
            .Where(g => g.Id == gameId &&
                    (
                        (g.GameMode == BalootGameMode.AnonymousGame && hasServiceAccountPermission) ||
                        (g.GameMode != BalootGameMode.AnonymousGame && (g.OwnerId == userId || g.ModeratorId == userId))
                    ))
            .ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(gameId, nameof(BalootGame)));
    }


}
