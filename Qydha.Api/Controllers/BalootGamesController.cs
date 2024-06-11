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
            .Resolve((game) => Ok(new { game.CreatedAt, game.Id, State = game.GameData }), HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.RegularUser)]

    [HttpPost("{gameId}/events")]
    public async Task<IActionResult> AddEventToGame([FromRoute] Guid gameId, [FromBody] List<BalootGameEventDto> eventsDtos)
    {
        User user = (User)HttpContext.Items["User"]!;
        List<BalootGameEvent> events = [];

        foreach (var eDto in eventsDtos)
        {
            Result<BalootGameEvent> res = eDto.MapToCorrespondingEvent();
            if (res.IsFailed)
                return res.Errors.First().Handle(HttpContext.TraceIdentifier);
            events.Add(res.Value);
        }

        return (await _balootGamesService
            .AddEvents(user, gameId, events))
            .Resolve((game) => Ok(new { game.Id, state=game.GameData, Message = "Events Added!" }), HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.RegularUser)]
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGameState([FromRoute] Guid gameId)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _balootGamesService
            .GetGameById(user, gameId))
            .Resolve((game) => Ok(new { game.Id, state=game.GameData }), HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpGet("{gameId}/statistics")]
    public async Task<IActionResult> GetGameStatistics([FromRoute] Guid gameId)
    {
        return (await _balootGamesService
            .GetGameStatisticsById(gameId))
            .Resolve((statistics) => Ok(new
            {
                Data = new { Statistics = statistics },
                Message = "Game Statistics fetched successfully."
            }), HttpContext.TraceIdentifier);
    }

    [Auth(SystemUserRoles.Admin)]
    [HttpGet("{gameId}/timeline")]
    public async Task<IActionResult> GetGameTimeLine([FromRoute] Guid gameId)
    {
        return (await _balootGamesService
            .GetGameTimeLineById(gameId))
            .Resolve((timeline) =>
                Ok(new
                {
                    Data = new { Timeline = timeline },
                    Message = "Game Timeline fetched successfully."
                }), HttpContext.TraceIdentifier);
    }
    [HttpGet("archive")]
    [Auth(SystemUserRoles.RegularUser)]

    public async Task<IActionResult> GetPagedArchive([FromQuery] PaginationParameters paginationParameters)
    {
        User user = (User)HttpContext.Items["User"]!;
        return (await _balootGamesService.GetUserArchive(user, paginationParameters))
            .Resolve((tuple) =>
            {
                return Ok(new
                {
                    Data = new BalootGameMapper().PageToGameArchiveDto(tuple.Games, tuple.WinsCount),
                    Message = "Archive Fetched."
                });
            }, HttpContext.TraceIdentifier);
    }
}