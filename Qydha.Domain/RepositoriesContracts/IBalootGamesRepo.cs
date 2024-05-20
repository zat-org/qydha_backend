namespace Qydha.Domain.Repositories;

public interface IBalootGamesRepo
{
    Task<Result<BalootGame>> CreateSingleBalootGame(Guid user);
    Task<Result<BalootGame>> GetById(Guid gameId);
    Task<Result> AddEvents(BalootGame game, ICollection<BalootGameEvent> events);
    Task<Result<PagedList<BalootGame>>> GetUserBalootGamesArchive(User user, PaginationParameters parameters);
    Task<Result<int>> GetUserBalootGamesWinsCount(User user);
}
