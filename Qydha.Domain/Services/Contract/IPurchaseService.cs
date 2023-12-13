namespace Qydha.Domain.Services.Contracts;

public interface IPurchaseService
{
    Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTime created_at);
    Task<Result<User>> SubscribeInFree(Guid userId, InfluencerCode? influencerCode);
    Task<Result<UserPromoCode>> AddPromoCodePurchase(UserPromoCode promoCode);
}
