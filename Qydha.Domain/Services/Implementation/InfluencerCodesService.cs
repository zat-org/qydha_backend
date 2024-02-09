
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IPurchaseService purchaseService, IInfluencerCodesCategoriesRepo influencerCodesCategoriesRepo) : IInfluencerCodesService
{
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly IInfluencerCodesCategoriesRepo _influencerCodesCategoriesRepo = influencerCodesCategoriesRepo;
    private readonly IPurchaseService _purchaseService = purchaseService;

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
            Result<int> getUsageNumRes = await _purchaseService.GetInfluencerCodeUsageByUserIdCountAsync(userId, influencerCode.Code);
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
            Result<int> getUsageNumRes = await _purchaseService.GetInfluencerCodeUsageByAllUsersCountAsync(influencerCode.Code);
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
            return (await _purchaseService.GetInfluencerCodeUsageCountByCategoryForUserAsync(userId, influencerCode.CategoryId.Value))
            .OnSuccessAsync(async (UsageNum) =>
            {
                return (await _influencerCodesCategoriesRepo.GetByIdAsync(influencerCode.CategoryId.Value))
                .MapTo((category) => new Tuple<int, InfluencerCodeCategory>(UsageNum, category));
            })
            .OnSuccess(tuple =>
            {
                int usageNum = tuple.Item1;
                int MaxUsageNum = tuple.Item2.MaxCodesPerUserInGroup;
                // TODO Send this Error Code ErrorType.InfluencerCodeCategoryAlreadyUsed
                if (usageNum >= MaxUsageNum)
                    return Result.Fail<InfluencerCode>(new()
                    {
                        Code = ErrorType.InvalidBodyInput,
                        Message = "عذرا لقد استخدمت كود آخر مشابه"
                    });
                return Result.Ok(influencerCode);
            });
        })
        .OnSuccessAsync(async (influencerCode) => await _purchaseService.AddInfluencerCodePurchase(influencerCode, userId));
    }
}
