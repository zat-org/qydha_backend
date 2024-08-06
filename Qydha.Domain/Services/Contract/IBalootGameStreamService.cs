namespace Qydha.Domain.Services.Contracts;

public interface IBalootGameStreamService
{
    Task<Result<BalootGameStreamEvent>> CreateBalootGameStreamEvent(Guid userId, BalootGame game);
    Task<Result<BalootGameStreamDto>> GetGameByBoardId(Guid boardId);
}