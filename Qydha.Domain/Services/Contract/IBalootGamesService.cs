namespace Qydha.Domain.Services.Contracts;

public interface IBalootGamesService
{
    Task<Result<BalootGame>> CreateSingleBalootGame(Guid userId, ICollection<BalootGameEvent> events, DateTimeOffset createdAt);
    Task<Result<BalootGame>> CreateAnonymousBalootGame(ICollection<BalootGameEvent> events, DateTimeOffset createdAt);
    Task<Result<BalootGame>> AddEvents(Guid userId, Guid gameId, ICollection<BalootGameEvent> events, bool hasPermission = false);
    Task<Result<BalootGame>> GetGameById(Guid userId, Guid gameId, bool isRequesterAdmin = false);
    Task<Result<List<BalootGameTimeLineBlock>>> GetGameTimeLineById(Guid gameId);
    Task<Result<(PagedList<BalootGame> Games, int WinsCount)>> GetUserArchive(Guid userId, PaginationParameters parameters);
    Task<Result<BalootGameStatistics>> GetGameStatisticsById(Guid gameId);
    Task<Result> DeleteByIds(Guid gameId, Guid userId, bool hasServiceAccountPermission = false);
}
