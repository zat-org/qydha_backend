namespace Qydha.Domain.Repositories;

public interface IUserPromoCodesRepo : IGenericRepository<UserPromoCode>
{
    Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId);
}
