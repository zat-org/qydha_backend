namespace Qydha.Domain.Repositories;

public interface IBalootGamesRepo
{
    Task<Result<BalootGame>> SaveGame(BalootGame game);
    Task<Result<BalootGame>> GetById(Guid gameId);
    Task<Result> DeleteByIds(Guid gameId, Guid userId, bool hasServiceAccountPermission = false);

    Task<Result> AddEvents(BalootGame game, ICollection<BalootGameEvent> events);
    Task<Result<PagedList<BalootGame>>> GetUserBalootGamesArchive(Guid userId, PaginationParameters parameters);
    Task<Result<int>> GetUserBalootGamesWinsCount(Guid userId);
    Task<Result<BalootGame>> GetBalootGameByBoardIdAsync(Guid boardId);

}