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
            return new ForbiddenError().Handle(HttpContext.TraceIdentifier);
        string tokenValue = authToken.ToString();
        if (tokenValue != _iAPHubSettings.XAuthToken)
            return new ForbiddenError().Handle(HttpContext.TraceIdentifier);

        if (webHookDto.Data!.IsSandbox)
        {
            _logger.LogWarning("Unhandled IAPHUB Action Type : {type} , Data => {data}", webHookDto.Type, webHookDto);
            return Ok();
        }
        switch (webHookDto.Type)
        {
            case "purchase":
                return (await _purchaseService.AddPurchase(webHookDto.Data!.Id, webHookDto.Data!.UserId, webHookDto.Data.ProductSku, webHookDto.Data.PurchaseDate))
                .Resolve(
                    (user) =>
                    {
                        var mapper = new UserMapper();
                        return Ok(new { Data = new { }, message = "Purchase Added Successfully." });
                    }, HttpContext.TraceIdentifier);
            case "refund":
                return (await _purchaseService.RefundPurchase(webHookDto.Data!.UserId, webHookDto.Data!.Id, webHookDto.Data.RefundDate))
                .Resolve(
                    (user) =>
                    {
                        var mapper = new UserMapper();
                        return Ok(new { Data = new { }, message = "Purchase Refunded Successfully." });
                    }, HttpContext.TraceIdentifier);
            default:
                _logger.LogWarning("Unhandled IAPHUB Action Type : {type} , Data => {data}", webHookDto.Type, webHookDto);
                return BadRequest();
        }
    }

}
