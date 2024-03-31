namespace Qydha.Domain.Repositories;

public interface IUserPromoCodesRepo
{

    Task<Result<UserPromoCode>> AddAsync(UserPromoCode code);
    Task<Result<UserPromoCode>> GetByIdAsync(Guid codeId);
    Task<Result> MarkAsUsedByIdAsync(Guid codeId);
    Task<Result<IEnumerable<UserPromoCode>>> GetUserValidPromoCodeAsync(Guid userId);
}
