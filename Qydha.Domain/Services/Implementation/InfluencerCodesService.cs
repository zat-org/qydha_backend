
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IPurchaseRepo purchaseRepo, INotificationService notificationService) : IInfluencerCodesService
{
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;

    public async Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTime? expireDate, int MaxInfluencedUsersCount)
    {
        var getCodeRes = await _influencerCodesRepo.IsCodeAvailable(code);
        return getCodeRes
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync<Guid>(new InfluencerCode(code, numOfDays, expireDate, MaxInfluencedUsersCount)));
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
        .OnSuccessAsync(async (influencerCode) =>
        {
            return (await _purchaseRepo.AddAsync<Guid>(new(influencerCode, userId)))
                    .OnSuccessAsync(async (purchase) =>
                         await _notificationService.SendToUserPreDefinedNotification(purchase.UserId, SystemDefaultNotifications.UseInfluencerCode));
        });
    }
}
