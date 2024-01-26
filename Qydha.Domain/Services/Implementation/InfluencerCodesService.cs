
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IPurchaseRepo purchaseRepo, INotificationService notificationService, IInfluencerCodesCategoriesRepo influencerCodesCategoriesRepo) : IInfluencerCodesService
{
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly IInfluencerCodesCategoriesRepo _influencerCodesCategoriesRepo = influencerCodesCategoriesRepo;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;

    public async Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTime? expireDate, int MaxInfluencedUsersCount, int? categoryId)
    {
        var getCodeRes = await _influencerCodesRepo.IsCodeAvailable(code);
        return getCodeRes
        .OnSuccessAsync(async () =>
        {
            if (categoryId is not null)
                return await _influencerCodesCategoriesRepo.GetByIdAsync(categoryId.Value);
            return Result.Ok();
        })
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync<Guid>(new InfluencerCode(code, numOfDays, expireDate, MaxInfluencedUsersCount, categoryId)));
    }

    public async Task<Result<User>> UseInfluencerCode(Guid userId, string code)
    {
        var getCodeRes = await _influencerCodesRepo.GetByCodeAsync(code);
        return getCodeRes
        .OnSuccess<InfluencerCode>((influencerCode) =>
        {
            if (influencerCode.ExpireAt is not null && influencerCode.ExpireAt.Value < DateTime.UtcNow)
                return Result.Fail<InfluencerCode>(new()
                {
                    Code = ErrorType.InfluencerCodeExpired,
                    Message = "Influencer Code Expired"
                });
            return Result.Ok(influencerCode);
        })
        .OnSuccessAsync<InfluencerCode>(async (influencerCode) =>
        {
            Result<int> getUsageNumRes = await _purchaseRepo.GetInfluencerCodeUsageByUserIdCountAsync(userId, influencerCode.Code);
            return getUsageNumRes.OnSuccess(num =>
            {
                if (num > 0)
                    return Result.Fail<InfluencerCode>(new()
                    {
                        Code = ErrorType.InfluencerCodeAlreadyUsed,
                        Message = "Influencer Code Used Before"
                    });
                return Result.Ok(influencerCode);
            });
        })
        .OnSuccessAsync<InfluencerCode>(async (influencerCode) =>
        {
            if (influencerCode.MaxInfluencedUsersCount == 0) return Result.Ok(influencerCode);
            Result<int> getUsageNumRes = await _purchaseRepo.GetInfluencerCodeUsageByAllUsersCountAsync(influencerCode.Code);
            return getUsageNumRes.OnSuccess(num =>
            {
                if (num >= influencerCode.MaxInfluencedUsersCount)
                    return Result.Fail<InfluencerCode>(new()
                    {
                        Code = ErrorType.InfluencerCodeExceedMaxUsageCount,
                        Message = "InfluencerCode Exceed Max Usage Count"
                    });
                return Result.Ok(influencerCode);
            });
        })
        .OnSuccessAsync<InfluencerCode>(async (influencerCode) =>
        {
            if (influencerCode.CategoryId is null) return Result.Ok(influencerCode);
            return (await _purchaseRepo.GetInfluencerCodeUsageCountByCategoryForUserAsync(userId, influencerCode.CategoryId.Value))
            .OnSuccessAsync(async (UsageNum) =>
            {
                return (await _influencerCodesCategoriesRepo.GetByIdAsync(influencerCode.CategoryId.Value))
                .MapTo((category) => new Tuple<int, InfluencerCodeCategory>(UsageNum, category));
            })
            .OnSuccess(tuple =>
            {
                int usageNum = tuple.Item1;
                int MaxUsageNum = tuple.Item2.MaxCodesPerUserInGroup;

                if (usageNum >= MaxUsageNum)
                    return Result.Fail<InfluencerCode>(new()
                    {
                        Code = ErrorType.InfluencerCodeCategoryAlreadyUsed,
                        Message = "User Exceed the max usage count of this influencer Code category"
                    });
                return Result.Ok(influencerCode);
            });
        })
        .OnSuccessAsync(async (influencerCode) =>
        {
            return (await _purchaseRepo.AddAsync<Guid>(new(influencerCode, userId)))
                    .OnSuccessAsync(async (purchase) =>
                         await _notificationService.SendToUserPreDefinedNotification(purchase.UserId, SystemDefaultNotifications.UseInfluencerCode));
        });
    }
}
