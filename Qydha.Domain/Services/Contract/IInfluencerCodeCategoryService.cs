namespace Qydha.Domain.Services.Contracts;

public interface IInfluencerCodeCategoryService
{
    Task<Result<InfluencerCodeCategory>> Add(InfluencerCodeCategory category);
    Task<Result<InfluencerCodeCategory>> Update(InfluencerCodeCategory category);
    Task<Result> Delete(int categoryId);
    Task<Result<IEnumerable<InfluencerCodeCategory>>> GetAll();
}
