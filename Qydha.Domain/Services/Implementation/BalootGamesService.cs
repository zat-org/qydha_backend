
namespace Qydha.Domain.Services.Implementation;

public class BalootGamesService(IBalootGamesRepo balootGamesRepo) : IBalootGamesService
{
    private readonly IBalootGamesRepo _balootGamesRepo = balootGamesRepo;
    public async Task<Result<BalootGame>> CreateSingleBalootGame(User owner)
    {
        return await _balootGamesRepo.CreateSingleBalootGame(owner.Id);
    }
    public async Task<Result<BalootGame>> AddEvents(User user, Guid gameId, ICollection<BalootGameEvent> events)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccessAsync<BalootGame>(async (game) =>
            {
                // ! change this check for the game moderator 
                if (user.Id != game.ModeratorId && user.Id != game.OwnerId)
                    return Result.Fail<BalootGame>(new()
                    {
                        Code = ErrorType.InvalidActionOrForbidden,
                        Message = "this user is not the moderator for this Game"
                    });
                return await _balootGamesRepo.AddEvents(game, events);
            });
    }
}
