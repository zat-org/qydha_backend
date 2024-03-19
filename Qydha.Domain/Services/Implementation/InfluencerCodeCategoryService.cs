

namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodeCategoryService(IInfluencerCodesCategoriesRepo influencerCodesCategoriesRepo) : IInfluencerCodeCategoryService
{
    private readonly IInfluencerCodesCategoriesRepo _influencerCodesCategoriesRepo = influencerCodesCategoriesRepo;
    public async Task<Result<InfluencerCodeCategory>> Add(InfluencerCodeCategory category)
    {
        return (await _influencerCodesCategoriesRepo.IsCategoryNameAvailableAsync(category.CategoryName))
        .OnSuccessAsync(async () => await _influencerCodesCategoriesRepo.Add(category));
    }

    public async Task<Result> Delete(int categoryId)
    {
        return await _influencerCodesCategoriesRepo.Delete(categoryId);
    }

    public async Task<Result<IEnumerable<InfluencerCodeCategory>>> GetAll()
    {
        return await _influencerCodesCategoriesRepo.GetAll();
    }

    public async Task<Result<InfluencerCodeCategory>> Update(InfluencerCodeCategory category)
    {
        return (await _influencerCodesCategoriesRepo.IsCategoryNameAvailableAsync(category.CategoryName, category.Id))
       .OnSuccessAsync(async () => await _influencerCodesCategoriesRepo.Update(category));
    }
}
