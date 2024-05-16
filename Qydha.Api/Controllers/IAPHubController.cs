namespace Qydha.API.Controllers;

[ApiController]
[Route("iaphub/")]
public class IAPHubController(IPurchaseService purchaseService, ILogger<IAPHubController> logger, IOptions<IAPHubSettings> iaphubSettings) : ControllerBase
{
    private readonly IPurchaseService _purchaseService = purchaseService;
    private readonly IAPHubSettings _iAPHubSettings = iaphubSettings.Value;
    private readonly ILogger<IAPHubController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> IApHubWebHook([FromBody] WebHookDto webHookDto)
    {
        // TODO
        if (!Request.Headers.TryGetValue("x-auth-token", out var authToken))
            return Unauthorized(new InvalidIAPHupTokenError());
        string tokenValue = authToken.ToString();
        if (tokenValue != _iAPHubSettings.XAuthToken)
            return Unauthorized(new InvalidIAPHupTokenError());

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
                        _logger.LogError("Error in IAPHUB WebHook type = \"purchase\" and  userId = {userId} with Data : {webhookData}", webHookDto.Data!.UserId, webHookDto);
                        return BadRequest(err);
                    }
                );
            default:
                _logger.LogWarning("Unhandled IAPHUB Action Type : {type} , Data => {data}", webHookDto.Type, webHookDto);
                return Ok();
        }
    }

}
