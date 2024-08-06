
using Qydha.Domain.Mappers;

namespace Qydha.Domain.Services.Implementation;

public class BalootGameStreamService(IUserRepo userRepo, IBalootGamesRepo balootGamesRepo) : IBalootGameStreamService
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IBalootGamesRepo _balootGamesRepo = balootGamesRepo;

    public async Task<Result<BalootGameStreamEvent>> CreateBalootGameStreamEvent(Guid userId, BalootGame game)
    {
        return (await _userRepo.GetBalootBoardByUserIdAsync(userId))
            .OnSuccess((board) =>
            {
                var gameDto = new BalootGameStreamMapper().BalootGameToBalootGameDto(game);
                return Result.Ok(new BalootGameStreamEvent(board.Id, gameDto, "StateChanged"));
            });
    }

    public async Task<Result<BalootGameStreamDto>> GetGameByBoardId(Guid boardId)
    {
        return (await _balootGamesRepo.GetBalootGameByBoardIdAsync(boardId))
            .OnSuccess((game) =>
            {
                var gameDto = new BalootGameStreamMapper().BalootGameToBalootGameDto(game);
                return Result.Ok(gameDto);
            });
    }
}
public record BalootGameStreamEvent(Guid BoardId, BalootGameStreamDto Game, string EventName);