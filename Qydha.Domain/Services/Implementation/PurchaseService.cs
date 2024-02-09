namespace Qydha.Domain.Services.Implementation;

public class PurchaseService(IPurchaseRepo purchaseRepo, IMediator mediator, IUserRepo userRepo, IOptions<SubscriptionSetting> subscriptionOptions, IOptions<ProductsSettings> productSettings) : IPurchaseService
{
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
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
            return (await _purchaseRepo.AddAsync<Guid>(purchase)).MapTo(tuple.Item1);
        })
        .OnSuccessAsync<User>(async (user) =>
        {
            await _mediator.Publish(new AddPurchaseNotification(user.Id, SystemDefaultNotifications.MakePurchase));
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> SubscribeInFree(Guid userId)
    {
        return (await _userRepo.GetByIdAsync(userId))
        .OnSuccessAsync<User>(async (user) =>
        {
            if (user.FreeSubscriptionUsed >= _subscriptionSetting.FreeSubscriptionsAllowed)
                return Result.Fail<User>(new()
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
            return (await _purchaseRepo.AddAsync<Guid>(purchase)).MapTo(user);
        })
        .OnSuccessAsync<User>(async (user) =>
        {
            await _mediator.Publish(new AddPurchaseNotification(user.Id, SystemDefaultNotifications.UseInfluencerCode));
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> AddPromoCodePurchase(UserPromoCode promoCode)
    {
        return (await _purchaseRepo.AddAsync<Guid>(new(promoCode)))
        .OnSuccessAsync(async (purchase) =>
        {
            await _mediator.Publish(new AddPurchaseNotification(purchase.UserId, SystemDefaultNotifications.UseTicket));
            return await _userRepo.GetByIdAsync(purchase.UserId);
        });
    }
    public async Task<Result<User>> AddInfluencerCodePurchase(InfluencerCode code, Guid userId)
    {
        return (await _purchaseRepo.AddAsync<Guid>(new(code, userId)))
       .OnSuccessAsync(async (purchase) =>
       {
           await _mediator.Publish(new AddPurchaseNotification(purchase.UserId, SystemDefaultNotifications.UseInfluencerCode));
           return await _userRepo.GetByIdAsync(purchase.UserId);
       });

    }

    public async Task<Result<int>> GetInfluencerCodeUsageByAllUsersCountAsync(string code) =>
        await _purchaseRepo.GetInfluencerCodeUsageByAllUsersCountAsync(code);

    public async Task<Result<int>> GetInfluencerCodeUsageCountByCategoryForUserAsync(Guid userId, int categoryId) =>
        await _purchaseRepo.GetInfluencerCodeUsageCountByCategoryForUserAsync(userId, categoryId);

    public async Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code) =>
            await _purchaseRepo.GetInfluencerCodeUsageByUserIdCountAsync(userId, code);
}
