namespace Qydha.Domain.Repositories;

public interface IUserPromoCodesRepo
{
    Task<Result<UserPromoCode>> AddAsync(UserPromoCode userPromoCode);
    Task<Result<UserPromoCode>> GetByIdAsync(Guid promoId);
    Task<Result> PatchById(Guid codeId, Dictionary<string, object> props);
    Task<Result> PatchById<T>(Guid codeId, string propName, T propValue);
    Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId);
}
