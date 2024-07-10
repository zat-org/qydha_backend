namespace Qydha.API.Controllers;
[ApiController]
[Route("baloot-games/")]
public class BalootGamesController(IBalootGamesService balootGamesService) : ControllerBase
{
    private readonly IBalootGamesService _balootGamesService = balootGamesService;

    [Authorize(Roles = RoleConstants.User)]
    [HttpPost]
    public IActionResult CreateBalootGame([FromBody] List<BalootGameEventDto> eventsDtos)
    {
        List<BalootGameEvent> events = [];
        foreach (var eDto in eventsDtos)
        {
            Result<BalootGameEvent> res = eDto.MapToCorrespondingEvent();
            if (res.IsFailed)
                return res.Errors.First().Handle(HttpContext.TraceIdentifier);
            events.Add(res.Value);
        }

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync((userId) => _balootGamesService.CreateSingleBalootGame(userId, events))
            .Resolve(
            (game) => Ok(new
            {
                Data = new BalootGameMapper().BalootGameToBalootGameDto(game),
                Message = "Game Created"
            })
            , HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpDelete("{gameId}")]
    public IActionResult DeleteBalootGame(Guid gameId)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async id => await _balootGamesService.DeleteById(gameId, id))
            .Resolve(
            () => Ok(new
            {
                Data = new { },
                Message = "Game Deleted Successfully"
            })
            , HttpContext.TraceIdentifier);
    }

    [Authorize(Roles = RoleConstants.User)]
    [HttpPost("{gameId}/events")]
    public IActionResult AddEventToGame([FromRoute] Guid gameId, [FromBody] List<BalootGameEventDto> eventsDtos)
    {
        List<BalootGameEvent> events = [];

        foreach (var eDto in eventsDtos)
        {
            Result<BalootGameEvent> res = eDto.MapToCorrespondingEvent();
            if (res.IsFailed)
                return res.Errors.First().Handle(HttpContext.TraceIdentifier);
            events.Add(res.Value);
        }

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async id => await _balootGamesService.AddEvents(id, gameId, events))
            .Resolve(
                (game) => Ok(new
                {
                    Data = new BalootGameMapper().BalootGameToBalootGameDto(game),
                    Message = "Events Added!"
                }), HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.SubscribedUser)]
    [HttpGet("{gameId}")]
    public IActionResult GetGameState([FromRoute] Guid gameId)
    {

        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async id => await _balootGamesService.GetGameById(id, gameId))
            .Resolve(
                (game) => Ok(new
                {
                    Data = new BalootGameMapper().BalootGameToBalootGameDto(game),
                    Message = "Game state fetched"
                }), HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.ServiceAccountPermission)]
    [Permission(ServiceAccountPermission.ReadBalootGameStatistics)]
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

    [Authorize(Policy = PolicyConstants.ServiceAccountPermission)]
    [Permission(ServiceAccountPermission.ReadBalootGameTimeline)]
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

    [Authorize(Policy = PolicyConstants.SubscribedUser)]
    [HttpGet("archive")]
    public IActionResult GetPagedArchive([FromQuery] PaginationParameters paginationParameters)
    {

        return HttpContext.User.GetUserIdentifier()
        .OnSuccessAsync(async id => await _balootGamesService.GetUserArchive(id, paginationParameters))
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