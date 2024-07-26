namespace Qydha.Domain.Services.Implementation;

public class PurchaseService(IPurchaseRepo purchaseRepo, IMediator mediator, IUserRepo userRepo, IOptions<ProductsSettings> productSettings, ILogger<PurchaseService> logger) : IPurchaseService
{
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
    private readonly ProductsSettings _productsSettings = productSettings.Value;
    private readonly ILogger<PurchaseService> _logger = logger;

    public async Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTimeOffset created_at)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        if (getUserRes.IsFailed)
        {
            _logger.LogCritical("trying to add purchase to not found user with id : {userId} and the purchase id is : {purchaseId}", userId, purchaseId);
            return Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
        }
        return getUserRes
        .OnSuccess((user) => _productsSettings.GetNumberOfDaysForProductSku(productSku, purchaseId).ToResult((numberOfDays) => (user, numberOfDays)))
        .OnSuccessAsync(async (tuple) =>
        {
            Purchase purchase = new()
            {
                IAPHubPurchaseId = purchaseId,
                UserId = tuple.user.Id,
                Type = "purchase",
                PurchaseDate = created_at,
                ProductSku = productSku,
                NumberOfDays = tuple.numberOfDays
            };
            return await _purchaseRepo.AddAsync(purchase);
        })
        .OnSuccessAsync(async (purchase) =>
        {
            await _mediator.Publish(new AddTransactionNotification(userId, TransactionType.Purchase));
            return await _userRepo.UpdateUserExpireDate(userId);
        });
    }
}

