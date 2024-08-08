namespace Qydha.Domain.Repositories;

public interface IPurchaseRepo
{
    Task<Result<Purchase>> AddAsync(Purchase purchase);
    Task<Result<IEnumerable<Purchase>>> GetAllByUserIdAsync(Guid userId);
    Task<Result<Purchase>> GetByPurchaseIdAsync(string purchaseId);
    Task<Result> RefundByPurchaseIdAsync(string purchaseId, DateTimeOffset refundedAt);
}
