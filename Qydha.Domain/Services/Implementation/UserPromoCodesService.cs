
namespace Qydha.Domain.Services.Implementation;

public class UserPromoCodesService(IUserPromoCodesRepo userPromoCodesRepo, INotificationService notificationService, IPurchaseRepo purchaseRepo, IUserRepo userRepo) : IUserPromoCodesService
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;
    private readonly INotificationService _notificationService = notificationService;

    private readonly IUserPromoCodesRepo _userPromoCodesRepo = userPromoCodesRepo;



    public async Task<Result<UserPromoCode>> AddPromoCode(Guid userId, string code, int numberOfDays, DateTime expireAt)
    {
        Result<User> getUserRes = await _userRepo.GetByIdAsync(userId);
        return getUserRes
        .OnSuccessAsync(async (user) => await _userPromoCodesRepo.AddAsync(new UserPromoCode(userId, code, numberOfDays, expireAt)))
        .OnSuccessAsync<UserPromoCode>(async (promo) =>
            (await _notificationService.SendToUser(Notification.CreatePromoCodeNotification(promo))).MapTo(promo)
        );
    }

    public async Task<Result<User>> UsePromoCode(Guid userId, Guid promoId)
    {
        Result<UserPromoCode> getPromoRes = await _userPromoCodesRepo.GetByIdAsync(promoId);
        return getPromoRes.OnSuccessAsync(async (promo) =>
               (await _userRepo.GetByIdAsync(promo.User_Id))
               .MapTo(user => new Tuple<User, UserPromoCode>(user, promo)))
       .OnSuccess<Tuple<User, UserPromoCode>>((tuple) =>
       {
           UserPromoCode promo = tuple.Item2;
           if (userId != promo.User_Id)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorCodes.AuthenticatedUserDoesNotOwnThisPromoCode,
                   Message = "Authenticated User does not own this Promo Code "
               });
           if (promo.Used_At is not null)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorCodes.PromoCodeAlreadyUsed,
                   Message = $"Promo Code Already Used before at : {promo.Used_At.Value.ToShortDateString()}"
               });

           if (promo.Expire_At.Date < DateTime.UtcNow.Date)
               return Result.Fail<Tuple<User, UserPromoCode>>(new()
               {
                   Code = ErrorCodes.PromoCodeExpired,
                   Message = "Promo Code Expired"
               });
           return Result.Ok(tuple);
       })
       .OnSuccessAsync(async (tuple) =>
       {
           UserPromoCode promo = tuple.Item2;

           Purchase purchase = new()
           {
               IAPHub_Purchase_Id = promo.Id.ToString(),
               User_Id = promo.User_Id,
               Type = "promo_code",
               Purchase_Date = DateTime.UtcNow,
               ProductSku = promo.Code,
               Number_Of_Days = promo.Number_Of_Days
           };
           return (await _purchaseRepo.AddAsync(purchase)).MapTo(promo);
       })
       .OnSuccessAsync<UserPromoCode>(async promo => (await _userPromoCodesRepo.PatchById(promo.Id, "Used_At", DateTime.UtcNow)).MapTo(promo))
       .OnSuccessAsync(async (promo) => await _userRepo.GetByIdAsync(promo.User_Id));
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetUserPromoCodes(Guid userId)
        => await _userPromoCodesRepo.GetAllUserValidPromoCodeAsync(userId);
}
