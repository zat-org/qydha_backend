namespace Qydha.Domain.Services.Implementation;

public class PurchaseService(IPurchaseRepo purchaseRepo, IUserRepo userRepo, INotificationService notificationService, IOptions<SubscriptionSetting> subscriptionOptions, IOptions<ProductsSettings> productSettings) : IPurchaseService
{
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly INotificationService _notificationService = notificationService;
    private readonly SubscriptionSetting _subscriptionSetting = subscriptionOptions.Value;
    private readonly ProductsSettings _productsSettings = productSettings.Value;

    public async Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTime created_at)
    {
        Result<User> getUserRes = (await _userRepo.GetByIdAsync(userId))
                    .OnFailure((err) => new()
                    {
                        Code = err.Code,
                        Message = $"user id provided in purchase = {userId} Not Found and Purchase id = {purchaseId}"
                    });

        return getUserRes.OnSuccess((user) =>
        {
            if (!_productsSettings.ProductsSku.TryGetValue(productSku, out int numberOfDays))
            {
                return Result.Fail<Tuple<User, int>>(new()
                {
                    Code = ErrorType.InvalidProductSku,
                    Message = $"Invalid ProductSku : '{productSku}' from Purchase with Id :{purchaseId}"
                });
            }
            return Result.Ok(new Tuple<User, int>(user, numberOfDays));
        })
        .OnSuccessAsync(async (tuple) =>
        {
            Purchase purchase = new()
            {
                IAPHubPurchaseId = purchaseId,
                UserId = tuple.Item1.Id,
                Type = "purchase",
                PurchaseDate = created_at,
                ProductSku = productSku,
                NumberOfDays = tuple.Item2
            };
            return await _purchaseRepo.AddAsync<Guid>(purchase);
        })
        .OnSuccessAsync(async (purchase) =>
            await _notificationService.SendToUserPreDefinedNotification(purchase.UserId, SystemDefaultNotifications.MakePurchase));
    }

    public async Task<Result<User>> SubscribeInFree(Guid userId)
    {
        return (await _userRepo.GetByIdAsync(userId))
        .OnSuccessAsync(async (user) =>
        {
            if (user.FreeSubscriptionUsed >= _subscriptionSetting.FreeSubscriptionsAllowed)
                return Result.Fail<Purchase>(new()
                {
                    Code = ErrorType.FreeSubscriptionExceededTheLimit,
                    Message = "Free Subscription Used by user Exceeded The Allowed Number"
                });
            var purchase = new Purchase()
            {
                IAPHubPurchaseId = Guid.NewGuid().ToString(),
                UserId = userId,
                Type = "free_30",
                PurchaseDate = DateTime.UtcNow,
                ProductSku = "free_30",
                NumberOfDays = _subscriptionSetting.NumberOfDaysInOneSubscription
            };
            return await _purchaseRepo.AddAsync<Guid>(purchase);
        })
        .OnSuccessAsync(async (purchase) =>
            await _notificationService.SendToUserPreDefinedNotification(purchase.UserId, SystemDefaultNotifications.UseInfluencerCode));
    }

    public async Task<Result<UserPromoCode>> AddPromoCodePurchase(UserPromoCode promoCode)
    {
        return (await _purchaseRepo.AddAsync<Guid>(new(promoCode)))
                .OnSuccessAsync(async (purchase) => await _notificationService.SendToUserPreDefinedNotification(purchase.UserId, SystemDefaultNotifications.UseTicket))
                .MapTo(promoCode);
    }

}
