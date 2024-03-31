
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IMediator mediator, IInfluencerCodesCategoriesRepo influencerCodesCategoriesRepo, IUserRepo userRepo) : IInfluencerCodesService
{
    private readonly IMediator _mediator = mediator;
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IInfluencerCodesCategoriesRepo _influencerCodesCategoriesRepo = influencerCodesCategoriesRepo;

    public async Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTimeOffset? expireDate, int MaxInfluencedUsersCount, int? categoryId)
    {
        var getCodeRes = await _influencerCodesRepo.IsCodeAvailable(code);
        return getCodeRes
        .OnSuccessAsync(async () =>
        {
            if (categoryId is not null)
                return await _influencerCodesCategoriesRepo.GetByIdAsync(categoryId.Value);
            return Result.Ok();
        })
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync(new InfluencerCode(code, numOfDays, expireDate, MaxInfluencedUsersCount, categoryId)));
    }

    public async Task<Result<User>> UseInfluencerCode(Guid userId, string code)
    {
        var getCodeRes = await _influencerCodesRepo.GetByCodeIfValidAsync(code);
        return getCodeRes
        .OnSuccessAsync<InfluencerCode>(async (influencerCode) =>
        {
            Result<int> getUsageNumRes = await _influencerCodesRepo.GetUserUsageCountByIdAsync(userId, influencerCode.Id);
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
            Result<int> getUsageNumRes = await _influencerCodesRepo.GetUsersUsageCountByIdAsync(influencerCode.Id);
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
            return (await _influencerCodesCategoriesRepo.GetUserUsageCountFromCategoryByIdAsync(userId, influencerCode.CategoryId.Value)).MapTo((usage) => new Tuple<int, InfluencerCode>(usage, influencerCode))
            .OnSuccess((tuple) =>
            {
                int usage = tuple.Item1;
                int MaxUsageNum = tuple.Item2.Category!.MaxCodesPerUserInGroup;
                if (usage >= MaxUsageNum)
                    return Result.Fail<InfluencerCode>(new()
                    {
                        Code = ErrorType.InvalidBodyInput,
                        Message = "عذرا لقد استخدمت كود آخر مشابه"
                    });
                return Result.Ok(influencerCode);
            });
        })
        .OnSuccessAsync<InfluencerCode>(async (influencerCode) => await _influencerCodesRepo.UseInfluencerCode(userId, influencerCode))
        .OnSuccessAsync(async (influencerCode) =>
        {
            await _mediator.Publish(new AddTransactionNotification(userId, TransactionType.InfluencerCode));
            return await _userRepo.UpdateUserExpireDate(userId);
        });
    }
}
