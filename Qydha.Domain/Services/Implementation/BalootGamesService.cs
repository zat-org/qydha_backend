
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
                if (user.Id != game.ModeratorId && user.Id != game.OwnerId)
                    return Result.Fail<BalootGame>(new()
                    {
                        Code = ErrorType.InvalidActionOrForbidden,
                        Message = "this user is not the moderator for this Game"
                    });


                // Apply Events To Game State ...
                foreach (var e in events)
                {
                    Result res = e.ApplyToState(game.State);
                    if (res.IsFailure)
                    {
                        return Result.Fail<BalootGame>(res.Error);
                    }
                }
                return (await _balootGamesRepo.AddEvents(game, events)).MapTo(game);
            });
    }

    public async Task<Result<BalootGame>> GetGameById(User Requester, Guid gameId)
    {
        return (await _balootGamesRepo.GetById(gameId))
            .OnSuccess<BalootGame>((game) =>
            {
                if (Requester.Id != game.ModeratorId && Requester.Id != game.OwnerId)
                    return Result.Fail<BalootGame>(new()
                    {
                        Code = ErrorType.InvalidActionOrForbidden,
                        Message = "this user is not the moderator for this Game"
                    });
                return Result.Ok(game);
            });
    }
}
