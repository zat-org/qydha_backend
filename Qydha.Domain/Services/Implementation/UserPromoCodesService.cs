
namespace Qydha.Domain.Services.Implementation;

public class UserPromoCodesService(IUserPromoCodesRepo userPromoCodesRepo, IMediator mediator, IUserRepo userRepo) : IUserPromoCodesService
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
    private readonly IUserPromoCodesRepo _userPromoCodesRepo = userPromoCodesRepo;

    public async Task<Result<UserPromoCode>> AddPromoCode(Guid userId, string code, int numberOfDays, DateTimeOffset expireAt)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccessAsync(async (user) => await _userPromoCodesRepo.AddAsync(new UserPromoCode(userId, code, numberOfDays, expireAt)))
        .OnSuccessAsync(async (promo) =>
        {
            await _mediator.Publish(new AddPromoCodeNotification(userId));
            return Result.Ok(promo);
        });
    }

    public async Task<Result<User>> UsePromoCode(Guid userId, Guid promoId)
    {
        return (await _userPromoCodesRepo.GetByIdAsync(promoId))
       .OnSuccess((promo) => promo.IsPromoCodeValidToUse(userId))
       .OnSuccessAsync(async () => await _userPromoCodesRepo.MarkAsUsedByIdAsync(promoId))
       .OnSuccessAsync(async () => await _userRepo.UpdateUserExpireDate(userId))
       .OnSuccessAsync(async (user) =>
        {
            await _mediator.Publish(new AddTransactionNotification(user, TransactionType.PromoCode));
            return Result.Ok(user);
        });

    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetUserValidPromoCodeAsync(Guid userId)
        => await _userPromoCodesRepo.GetUserValidPromoCodeAsync(userId);
}
