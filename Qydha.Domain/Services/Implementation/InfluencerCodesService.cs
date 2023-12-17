
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IPurchaseRepo purchaseRepo, INotificationService notificationService) : IInfluencerCodesService
{
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IPurchaseRepo _purchaseRepo = purchaseRepo;

    public async Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTime? expireDate)
    {
        var getCodeRes = await _influencerCodesRepo.IsCodeAvailable(code);
        return getCodeRes
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync<Guid>(new InfluencerCode(code, numOfDays, expireDate)));
    }

    public async Task<Result<User>> UseInfluencerCode(Guid userId, string code)
    {

        var getCodeRes = await _influencerCodesRepo.GetByCodeAsync(code);
        return getCodeRes
        .OnSuccess<InfluencerCode>((influencerCode) =>
        {
            if (influencerCode.ExpireAt > DateTime.UtcNow)
                return Result.Fail<InfluencerCode>(new()
                {
                    Code = ErrorCodes.InfluencerCodeExpired,
                    Message = "Influencer Code Expired"
                });
            return Result.Ok(influencerCode);
        })
        .OnSuccessAsync(async (influencerCode) =>
        {
            Purchase purchase = new()
            {
                IAPHubPurchaseId = influencerCode.Id.ToString(),
                UserId = userId,
                Type = "Influencer",
                PurchaseDate = DateTime.UtcNow,
                ProductSku = influencerCode.Code,
                NumberOfDays = influencerCode.NumberOfDays
            };
            return (await _purchaseRepo.AddAsync<Guid>(purchase))
                    .OnSuccessAsync(async (purchase) =>
                        await _notificationService.SendToUser(Notification.CreatePurchaseNotification(purchase)));
        });
    }
}
