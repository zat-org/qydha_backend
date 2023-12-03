namespace Qydha.Api.Controllers;

[ApiController]
[Route("iaphub/")]
public class IAPHubController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly IAPHubSettings _iAPHubSettings;
    private readonly ILogger<IAPHubController> _logger;

    public IAPHubController(IPurchaseService purchaseService, ILogger<IAPHubController> logger, IOptions<IAPHubSettings> iaphubSettings)
    {
        _iAPHubSettings = iaphubSettings.Value;
        _purchaseService = purchaseService;
        _logger = logger;
    }
    [HttpPost]
    public async Task<IActionResult> IApHubWebHook([FromBody] WebHookDto webHookDto)
    {
        if (!Request.Headers.TryGetValue("x-auth-token", out var authToken))
            return Unauthorized(new { Error = new Error() { Code = ErrorCodes.InvalidIAPHupToken, Message = "x-auth-token header is Missing" } });
        string tokenValue = authToken.ToString();
        if (tokenValue != _iAPHubSettings.XAuthToken) return Unauthorized(new { Error = new Error() { Code = ErrorCodes.InvalidIAPHupToken, Message = "x-auth-token header is wrong." } });

        switch (webHookDto.Type)
        {
            case "purchase":
                return (await _purchaseService.AddPurchase(webHookDto.Id, webHookDto.Data!.UserId, webHookDto.Data.ProductSku, webHookDto.Data.PurchaseDate)).Handle<User, IActionResult>(
                    (user) =>
                    {
                        var mapper = new UserMapper();
                        return Ok(new { Data = new { user = mapper.UserToUserDto(user) }, message = "Enjoy your subscription." });
                    },
                    (err) =>
                    {
                        _logger.LogError(err.ToString());
                        return BadRequest(err);
                    }
                );
            default:
                _logger.LogWarning($"Unhandled IAPHUB Action Type : {webHookDto.Type} , Data => {webHookDto}");
                return Ok();
        }
    }

    [Authorize]
    [TypeFilter(typeof(AuthFilter))]
    [HttpPost("free_30/")]
    public async Task<IActionResult> SubscribeInFree()
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;
        return (await _purchaseService.SubscribeInFree(userId))
        .Handle<User, IActionResult>(
            (user) =>
            {
                var mapper = new UserMapper();
                return Ok(new { Data = new { user = mapper.UserToUserDto(user) }, message = "Enjoy your free subscription." });
            },
            BadRequest);
    }

}
