namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesCategoriesRepo(QydhaContext qydhaContext , ILogger<InfluencerCodesCategoriesRepo> logger) :  IInfluencerCodesCategoriesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext ;
    private readonly ILogger<InfluencerCodesCategoriesRepo> _logger = logger ;
    public async Task<Result<InfluencerCodeCategory>> GetByIdAsync(int id) {
         return  await _dbCtx.InfluencerCodeCategories.FirstOrDefaultAsync(code => code.Id == id) is InfluencerCodeCategory category ?
            Result.Ok(category) :
            Result.Fail<InfluencerCodeCategory> (new() {
                Code = ErrorType.InfluencerCodeCategoryNotFound,
                Message = "Influencer Code Category NotFound :: Entity Not Found"
            });
    }

    public async Task<Result<InfluencerCodeCategory>> GetByCategoryNameAsync(string categoryName) 
    {
         return  await _dbCtx.InfluencerCodeCategories.FirstOrDefaultAsync(code => code.CategoryName == categoryName.ToUpper()) is InfluencerCodeCategory category ?
            Result.Ok(category) :
            Result.Fail<InfluencerCodeCategory> (new() {
                Code = ErrorType.InfluencerCodeCategoryNotFound,
                Message = "Influencer Code Category NotFound :: Entity Not Found"
            });
    }

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

    public async Task<Result<InfluencerCodeCategory>> Add(InfluencerCodeCategory category)
    {
        await _dbCtx.InfluencerCodeCategories.AddAsync(category);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(category);
    }

    public async Task<Result<InfluencerCodeCategory>> Update(InfluencerCodeCategory category)
    {
        var affected = await _dbCtx.InfluencerCodeCategories.Where(c=> c.Id == category.Id).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(c=> c.MaxCodesPerUserInGroup , category.MaxCodesPerUserInGroup)
                .SetProperty(c=> c.CategoryName , category.CategoryName)
            );
        return affected == 1 ? 
            Result.Ok(category) : 
            Result.Fail<InfluencerCodeCategory>(new Error(){
                Code = ErrorType.InfluencerCodeCategoryNotFound,
                Message = "Influencer Code Category Not Found :: Entity Not Found"
            });
    }

    public async Task<Result> Delete(int categoryId)
    {
        var affected = await _dbCtx.InfluencerCodeCategories.Where(c=> c.Id == categoryId).ExecuteDeleteAsync();
        return affected == 1 ? 
            Result.Ok() : 
            Result.Fail<InfluencerCodeCategory>(new Error(){
                Code = ErrorType.InfluencerCodeCategoryNotFound,
                Message = "Influencer Code Category Not Found :: Entity Not Found"
            });
    }

    public async Task<Result<IEnumerable<InfluencerCodeCategory>>> GetAll()
    {
        List<InfluencerCodeCategory> categories = await _dbCtx.InfluencerCodeCategories.Include(c=>c.InfluencerCodes).ToListAsync();
        return Result.Ok((IEnumerable<InfluencerCodeCategory>)categories);
    }
    
}