namespace Qydha.API.Controllers;
[ApiController]
[Route("baloot-games/")]
public class BalootGamesController(IBalootGamesService balootGamesService) : ControllerBase
{
    private readonly IBalootGamesService _balootGamesService = balootGamesService;
    [Auth(SystemUserRoles.RegularUser)]

    [HttpPost]
    public async Task<IActionResult> CreateBalootGame()
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _balootGamesService.CreateSingleBalootGame(user))
            .Handle<BalootGame, IActionResult>(
                (game) => Ok(new { game.CreatedAt, game.Id, game.State }),
                BadRequest
            );
    }

    [Auth(SystemUserRoles.RegularUser)]

    [HttpPost("{gameId}/events")]
    public async Task<IActionResult> AddEventToGame([FromRoute] Guid gameId, [FromBody] List<BalootGameEventDto> eventsDtos)
    {
        User user = (User)HttpContext.Items["User"]!;
        var events = eventsDtos.Select(dto => dto.MapToCorrespondingEvent()).ToList();
        return (await _balootGamesService
            .AddEvents(user, gameId, events))
            .Handle<BalootGame, IActionResult>(
                (game) => Ok(new { game.Id, game.State, Message = "Events Added!" }),
                BadRequest
            );
    }

    [Auth(SystemUserRoles.RegularUser)]
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGameState([FromRoute] Guid gameId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _balootGamesService
            .GetGameById(user, gameId))
            .Handle<BalootGame, IActionResult>(
                (game) => Ok(new { game.Id, game.State }),
                BadRequest
            );
    }
}