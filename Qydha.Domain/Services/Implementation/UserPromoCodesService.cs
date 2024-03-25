
namespace Qydha.Domain.Services.Implementation;

public class UserPromoCodesService(IUserPromoCodesRepo userPromoCodesRepo, IMediator mediator, IPurchaseService purchaseService, IUserRepo userRepo) : IUserPromoCodesService
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMediator _mediator = mediator;
    private readonly IPurchaseService _purchaseService = purchaseService;
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
       .OnSuccessAsync<Tuple<User, UserPromoCode>>(async (tuple) => (await _purchaseService.AddPromoCodePurchase(tuple.Item2))
            .MapTo((user) => new Tuple<User, UserPromoCode>(user, tuple.Item2)))
       .OnSuccessAsync(async tuple => (await _userPromoCodesRepo.MarkAsUsedByIdAsync(tuple.Item2.Id)).MapTo(tuple.Item1));
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetUserPromoCodes(Guid userId)
        => await _userPromoCodesRepo.GetAllUserValidPromoCodeAsync(userId);
}
