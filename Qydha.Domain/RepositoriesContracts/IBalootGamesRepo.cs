namespace Qydha.Domain.Repositories;

public interface IBalootGamesRepo
{
    Task<Result<BalootGame>> CreateSingleBalootGame(Guid user);
    Task<Result<BalootGame>> GetById(Guid gameId);
    Task<Result<BalootGame>> AddEvents(BalootGame game, ICollection<BalootGameEvent> events);
}
