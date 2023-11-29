namespace Qydha.Domain.Services.Contracts;

public interface IPurchaseService
{
    Task AddPurchase(Purchase purchase);
    Task<Result<User>> SubscribeInFree(Guid userId);
}
