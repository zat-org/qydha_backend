namespace Qydha.Domain.Repositories;

public interface IPurchaseRepo
{
    Task<Result<Purchase>> AddAsync(Purchase purchase);
    Task<Result<int>> GetInfluencerCodeUsageByUserIdCountAsync(Guid userId, string code);
    Task<Result<IEnumerable<Purchase>>> GetAllByUserIdAsync(Guid userId);
    Task<Result<int>> GetInfluencerCodeUsageByAllUsersCountAsync(string code);
    Task<Result<int>> GetInfluencerCodeUsageCountByCategoryForUserAsync(Guid userId, int categoryId);
}
