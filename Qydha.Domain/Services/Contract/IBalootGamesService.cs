namespace Qydha.Domain.Services.Contracts;

public interface IBalootGamesService
{
    Task<Result<BalootGame>> CreateSingleBalootGame(Guid userId, ICollection<BalootGameEvent> events);
    Task<Result<BalootGame>> AddEvents(Guid userId, Guid gameId, ICollection<BalootGameEvent> events);
    Task<Result<BalootGame>> GetGameById(Guid userId, Guid gameId);
    Task<Result<List<BalootGameTimeLineBlock>>> GetGameTimeLineById(Guid gameId);
    Task<Result<(PagedList<BalootGame> Games, int WinsCount)>> GetUserArchive(Guid userId, PaginationParameters parameters);
    Task<Result<BalootGameStatistics>> GetGameStatisticsById(Guid gameId);
    Task<Result> DeleteById(Guid gameId, Guid userId);
}
