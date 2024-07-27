namespace Qydha.API.Controllers;
[ApiController]
[Route("baloot-games/")]
public class BalootGamesController(IBalootGamesService balootGamesService) : ControllerBase
{
    private readonly IBalootGamesService _balootGamesService = balootGamesService;

    [Authorize(Policy = PolicyConstants.UserOrServiceAccount)]
    [Permission(ServiceAccountPermission.AnonymousBalootGameCRUDs)]
    [HttpPost]
    public IActionResult CreateBalootGame([ModelBinder(typeof(CreateBalootGameDtoModelBinder))] CreateBalootGameDto dto)
    {
        var xInfoData = HttpContext.Request.Headers.GetXInfoHeaderData();
        var balootGameMapper = new BalootGameMapper();
        return balootGameMapper.BalootEventsDtoToBalootEvents(dto.Events)
        .OnSuccess(events => HttpContext.User.GetUserIdentifier().Map(userId => (userId, events)))
        .OnSuccessAsync((tuple) =>
           {
               if (HttpContext.User.IsServiceAccountToken())
                   return _balootGamesService.CreateAnonymousBalootGame(tuple.events, dto.CreatedAt, xInfoData);
               return _balootGamesService.CreateSingleBalootGame(tuple.userId, tuple.events, dto.CreatedAt, xInfoData);
           })
           .Resolve(
           (game) => Ok(new
           {
               Data = balootGameMapper.BalootGameToBalootGameDto(game),
               Message = "Game Created"
           })
           , HttpContext.TraceIdentifier);
    }


    [Authorize(Policy = PolicyConstants.UserOrServiceAccount)]
    [Permission(ServiceAccountPermission.AnonymousBalootGameCRUDs)]
    [HttpDelete("{gameId}")]
    public IActionResult DeleteBalootGame(Guid gameId)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async id =>
                await _balootGamesService.DeleteByIds(gameId, id, hasServiceAccountPermission: HttpContext.User.IsServiceAccountToken()))
            .Resolve(
            () => Ok(new
            {
                Data = new { },
                Message = "Game Deleted Successfully"
            })
            , HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.UserOrServiceAccount)]
    [Permission(ServiceAccountPermission.AnonymousBalootGameCRUDs)]
    [HttpPost("{gameId}/events")]
    public IActionResult AddEventToGame([FromRoute] Guid gameId, [ModelBinder(typeof(BalootGameEventDtoListModelBinder))] List<BalootGameEventDto> eventsDtos)
    {
        var balootGameMapper = new BalootGameMapper();
        return balootGameMapper.BalootEventsDtoToBalootEvents(eventsDtos)
        .OnSuccess(events => HttpContext.User.GetUserIdentifier().Map(userId => (userId, events)))
        .OnSuccessAsync(async (tuple) =>
        {
            if (HttpContext.User.IsServiceAccountToken())
                return await _balootGamesService.AddEvents(tuple.userId, gameId, tuple.events, hasPermission: true);
            return await _balootGamesService.AddEvents(tuple.userId, gameId, tuple.events);
        })
        .Resolve(
        (game) => Ok(new
        {
            Data = balootGameMapper.BalootGameToBalootGameDto(game),
            Message = "Events Added!"
        }), HttpContext.TraceIdentifier);
    }

    [Authorize(Policy = PolicyConstants.AdminOrSubscribedUser)]
    [HttpGet("{gameId}")]
    public IActionResult GetGameState([FromRoute] Guid gameId)
    {
        return HttpContext.User.GetUserIdentifier()
            .OnSuccessAsync(async userId =>
                await _balootGamesService.GetGameById(userId, gameId, isRequesterAdmin: HttpContext.User.HasAdminRole()))
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