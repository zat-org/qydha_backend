namespace Qydha.Domain.Repositories;

public interface IInfluencerCodesCategoriesRepo : IGenericRepository<InfluencerCodeCategory>
{
    Task<Result<InfluencerCodeCategory>> GetByIdAsync(int id);
    Task<Result<InfluencerCodeCategory>> GetByCategoryNameAsync(string categoryName);
    Task<Result> IsCategoryNameAvailableAsync(string categoryName, int? categoryId = null);
}
