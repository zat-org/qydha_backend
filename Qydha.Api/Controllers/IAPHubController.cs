namespace Qydha.Api.Controllers;

[ApiController]
[Route("iaphub/")]
public class IAPHubController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly IAPHubSettings _iAPHubSettings;
    private readonly ProductsSettings _productsSettings;
    private readonly ILogger<IAPHubController> _logger;

    public IAPHubController(IPurchaseService purchaseService, ILogger<IAPHubController> logger, IOptions<IAPHubSettings> iaphubSettings, IOptions<ProductsSettings> productSettings)
    {
        _iAPHubSettings = iaphubSettings.Value;
        _purchaseService = purchaseService;
        _productsSettings = productSettings.Value;
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
                if (!_productsSettings.ProductsSku.TryGetValue(webHookDto.Data!.ProductSku, out int numberOfDays))
                {
                    _logger.LogWarning($"Invalid ProductSku {webHookDto.Data!.ProductSku} from Purchase ");
                    // return BadRequest(new Error() { Code = ErrorCodes.InvalidInput, Message = "Invalid Product sku" });
                }

                var purchase = new Purchase()
                {
                    IAPHub_Purchase_Id = webHookDto.Data!.Id,
                    User_Id = webHookDto.Data!.UserId,
                    Type = webHookDto.Type,
                    Purchase_Date = webHookDto.Data!.PurchaseDate,
                    ProductSku = webHookDto.Data!.ProductSku,
                    Number_Of_Days = numberOfDays
                };

                await _purchaseService.AddPurchase(purchase);
                return Ok();
            default:
                _logger.LogWarning($"Unhandled IAPHUB Action Type : {webHookDto.Type}", webHookDto);
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
                return Ok(new { Data = mapper.UserToUserDto(user) });
            },
            BadRequest);
    }

}
