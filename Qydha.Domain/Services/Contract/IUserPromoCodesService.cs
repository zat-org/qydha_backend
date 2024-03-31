namespace Qydha.Domain.Services.Contracts;

public interface IUserPromoCodesService
{
    Task<Result<UserPromoCode>> AddPromoCode(Guid userId, string code, int numberOfDays, DateTimeOffset expireAt);
    Task<Result<User>> UsePromoCode(Guid userId, Guid promoId);
    Task<Result<IEnumerable<UserPromoCode>>> GetUserValidPromoCodeAsync(Guid userId);
}
