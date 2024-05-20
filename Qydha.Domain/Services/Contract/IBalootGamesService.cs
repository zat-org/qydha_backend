namespace Qydha.Domain.Services.Contracts;

public interface IBalootGamesService
{
    Task<Result<BalootGame>> CreateSingleBalootGame(User owner);
    Task<Result<BalootGame>> AddEvents(User moderator, Guid gameId, ICollection<BalootGameEvent> events);
    Task<Result<BalootGame>> GetGameById(User Requester, Guid gameId);
    Task<Result<List<BalootGameTimeLineBlock>>> GetGameTimeLineById(Guid gameId);
    Task<Result<(PagedList<BalootGame> Games, int WinsCount)>> GetUserArchive(User user, PaginationParameters parameters);
    Task<Result<BalootGameStatistics>> GetGameStatisticsById(Guid gameId);
}
