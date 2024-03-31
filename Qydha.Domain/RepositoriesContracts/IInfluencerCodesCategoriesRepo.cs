namespace Qydha.Domain.Repositories;

public interface IInfluencerCodesCategoriesRepo
{
    Task<Result<InfluencerCodeCategory>> GetByIdAsync(int id);
    Task<Result<InfluencerCodeCategory>> GetByCategoryNameAsync(string categoryName);
    Task<Result> IsCategoryNameAvailableAsync(string categoryName, int? categoryId = null);
    Task<Result<InfluencerCodeCategory>> Add(InfluencerCodeCategory category);
    Task<Result<InfluencerCodeCategory>> Update(InfluencerCodeCategory category);
    Task<Result> Delete(int categoryId);
    Task<Result<IEnumerable<InfluencerCodeCategory>>> GetAll();
    Task<Result<int>> GetUserUsageCountFromCategoryByIdAsync(Guid userId, int categoryId);

}
