namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesCategoriesRepo(IDbConnection dbConnection, ILogger<InfluencerCodesCategoriesRepo> logger) : GenericRepository<InfluencerCodeCategory>(dbConnection, logger), IInfluencerCodesCategoriesRepo
{
    public async Task<Result<InfluencerCodeCategory>> GetByIdAsync(int id) =>
         await GetByUniquePropAsync(nameof(InfluencerCodeCategory.Id), id);

    public async Task<Result<InfluencerCodeCategory>> GetByCategoryNameAsync(string categoryName) =>
        await GetByUniquePropAsync(nameof(InfluencerCodeCategory.CategoryName), categoryName.ToUpper());

    public async Task<Result> IsCategoryNameAvailableAsync(string categoryName, int? categoryId = null)
    {
        Result<InfluencerCodeCategory> getCategoryRes = await GetByCategoryNameAsync(categoryName);
        if (getCategoryRes.IsSuccess && (categoryId is null || (categoryId is not null && categoryId != getCategoryRes.Value.Id)))
            return Result.Fail(new()
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "هذا التصنيف موجود بالفعل"
            });
        return Result.Ok();
    }
}
