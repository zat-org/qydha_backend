namespace Qydha.Domain.Services.Contracts;

public interface IPurchaseService
{
    Task<Result<User>> AddPurchase(string purchaseId, Guid userId, string productSku, DateTimeOffset created_at);
    Task<Result<User>> RefundPurchase(Guid userId, string purchaseId, DateTimeOffset refundedAt);

}
