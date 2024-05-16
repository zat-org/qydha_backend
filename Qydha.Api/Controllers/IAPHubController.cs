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
        if (!Request.Headers.TryGetValue("x-auth-token", out var authToken))
            return new ForbiddenError().Handle();
        string tokenValue = authToken.ToString();
        if (tokenValue != _iAPHubSettings.XAuthToken)
            return new ForbiddenError().Handle();

        switch (webHookDto.Type)
        {
            case "purchase":
                return (await _purchaseService.AddPurchase(webHookDto.Id, webHookDto.Data!.UserId, webHookDto.Data.ProductSku, webHookDto.Data.PurchaseDate))
                .Resolve(
                    (user) =>
                    {
                        var mapper = new UserMapper();
                        return Ok(new { Data = new { }, message = "Purchase Added Successfully." });
                    });
            default:
                _logger.LogWarning("Unhandled IAPHUB Action Type : {type} , Data => {data}", webHookDto.Type, webHookDto);
                return BadRequest();
        }
    }

}
