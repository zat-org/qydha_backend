
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
        .OnSuccessAsync<UserPromoCode>(async (promo) =>
        {
            await _mediator.Publish(new AddPromoCodeNotification(userId));
            return Result.Ok(promo);
        });
    }

    public async Task<Result<User>> UsePromoCode(Guid userId, Guid promoId)
    {
        Result<UserPromoCode> getPromoRes = await _userPromoCodesRepo.GetByIdAsync(promoId);
        return getPromoRes.OnSuccessAsync(async (promo) =>
               (await _userRepo.GetByIdAsync(promo.UserId))
               .MapTo(user => new Tuple<User, UserPromoCode>(user, promo)))
       .OnSuccess<Tuple<User, UserPromoCode>>((tuple) =>
       {
           UserPromoCode promo = tuple.Item2;
           if (userId != promo.UserId)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorType.UserDoesNotOwnThePromoCode,
                   Message = "Authenticated User does not own this Promo Code "
               });
           if (promo.UsedAt is not null)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorType.PromoCodeAlreadyUsed,
                   Message = $"Promo Code Already Used before at : {promo.UsedAt.Value.ToLocalTime()}"
               });

           if (promo.ExpireAt.Date < DateTimeOffset.UtcNow.Date)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorType.PromoCodeExpired,
                   Message = "Promo Code Expired"
               });
           return Result.Ok(tuple);
       })
       .OnSuccessAsync(async tuple =>
        {
            User user = tuple.Item1;
            UserPromoCode promo = tuple.Item2;
            return (await _userPromoCodesRepo.MarkAsUsedByIdAsync(promo.Id)).MapTo(user);
        }).OnSuccessAsync<User>(async (user) =>
        {
            await _mediator.Publish(new AddTransactionNotification(user.Id, TransactionType.PromoCode));
            return await _userRepo.UpdateUserExpireDate(userId);
        });
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetUserValidPromoCodeAsync(Guid userId)
        => await _userPromoCodesRepo.GetUserValidPromoCodeAsync(userId);
}
