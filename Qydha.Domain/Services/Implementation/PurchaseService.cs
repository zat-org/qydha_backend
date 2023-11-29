namespace Qydha.Domain.Services.Implementation;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepo _purchaseRepo;
    private readonly IUserRepo _userRepo;
    private readonly SubscriptionSetting _subscriptionSetting;

    public PurchaseService(IPurchaseRepo purchaseRepo, IUserRepo userRepo, IOptions<SubscriptionSetting> subscriptionOptions)
    {
        _purchaseRepo = purchaseRepo;
        _subscriptionSetting = subscriptionOptions.Value;
        _userRepo = userRepo;
    }
    public async Task AddPurchase(Purchase purchase)
    {
        await _purchaseRepo.AddAsync(purchase);
    }

    public async Task<Result<User>> SubscribeInFree(Guid userId)
    {
        return (await _userRepo.GetByIdAsync(userId))
        .OnSuccessAsync(async (user) =>
        {
            if (user.Free_Subscription_Used >= _subscriptionSetting.FreeSubscriptionsAllowed)
                return Result.Fail<Purchase>(new()
                {
                    Code = ErrorCodes.FreeSubscriptionUsedExceededTheAllowedNumber,
                    Message = "Free Subscription Used by user Exceeded The Allowed Number"
                });
            var purchase = new Purchase()
            {
                IAPHub_Purchase_Id = Guid.NewGuid().ToString(),
                User_Id = userId,
                Type = "purchase",
                Purchase_Date = DateTime.Now,
                ProductSku = _subscriptionSetting.FreeSubscriptionName,
                Number_Of_Days = _subscriptionSetting.NumberOfDaysInOneSubscription
            };
            return await _purchaseRepo.AddAsync(purchase);
        }).OnSuccessAsync(async (purchase) => await _userRepo.GetByIdAsync(purchase.User_Id));
    }
}
