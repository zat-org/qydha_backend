namespace Qydha.Domain.Services.Contracts;

public interface IUserPromoCodesService
{
    Task<Result<UserPromoCode>> AddPromoCode(Guid userId, string code, int numberOfDays, DateTime expireAt);
    Task<Result<User>> UsePromoCode(Guid userId, Guid promoId);
    Task<Result<IEnumerable<UserPromoCode>>> GetUserPromoCodes(Guid userId);
}
