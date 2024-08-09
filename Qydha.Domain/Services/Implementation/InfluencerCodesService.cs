
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
        return getCodeRes.OnSuccessAsync(async () =>
        {
            if (categoryId != null)
                return (await _influencerCodesCategoriesRepo.GetByIdAsync(categoryId.Value)).ToResult();
            return Result.Ok();
        })
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync(new InfluencerCode(code, numOfDays, expireDate, MaxInfluencedUsersCount, categoryId)));
    }

    public async Task<Result<User>> UseInfluencerCode(Guid userId, string code)
    {
        var getCodeRes = await _influencerCodesRepo.GetByCodeIfValidAsync(code);
        return getCodeRes
        .OnSuccessAsync(async (influencerCode) =>
        {
            return (await _influencerCodesRepo.GetUserUsageCountByIdAsync(userId, influencerCode.Id))
                .OnSuccess(usageCount => influencerCode.IsUserReachLimit(usageCount)).ToResult(influencerCode);
        })
        .OnSuccessAsync(async (influencerCode) =>
        {
            if (influencerCode.MaxInfluencedUsersCount == 0) return Result.Ok(influencerCode);
            return (await _influencerCodesRepo.GetUsersUsageCountByIdAsync(influencerCode.Id))
                .OnSuccess(usageCount => influencerCode.IsUsersReachedMaxUsage(usageCount)).ToResult(influencerCode);
        })
        .OnSuccessAsync(async (influencerCode) =>
        {
            if (influencerCode.CategoryId is null) return Result.Ok(influencerCode);
            return (await _influencerCodesCategoriesRepo.GetUserUsageCountFromCategoryByIdAsync(userId, influencerCode.CategoryId.Value))
                .ToResult((usage) => (usage, influencerCode))
            .OnSuccess((tuple) => tuple.influencerCode.IsUserReachedCategoryMaxUsage(tuple.usage).ToResult(influencerCode));
        })
        .OnSuccessAsync(async (influencerCode) => await _influencerCodesRepo.UseInfluencerCode(userId, influencerCode))
        .OnSuccessAsync(async (influencerCode) => await _userRepo.UpdateUserExpireDate(userId))
        .OnSuccessAsync(async (user) =>
        {
            await _mediator.Publish(new AddTransactionNotification(user, TransactionType.InfluencerCode));
            await _mediator.Publish(new UserDataChangedNotification(user));
            return Result.Ok(user);
        });
    }
}
