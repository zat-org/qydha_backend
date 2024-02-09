namespace Qydha.Domain.Services.Contracts;

public interface IPurchaseService
{
    Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTime created_at);
    Task<Result<User>> SubscribeInFree(Guid userId);
    Task<Result<User>> AddPromoCodePurchase(UserPromoCode promoCode);
    Task<Result<User>> AddInfluencerCodePurchase(InfluencerCode code, Guid userId);
    Task<Result<int>> GetInfluencerCodeUsageByAllUsersCountAsync(string code);
    Task<Result<int>> GetInfluencerCodeUsageCountByCategoryForUserAsync(Guid userId, int categoryId);
    Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code);

}
