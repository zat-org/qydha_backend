
namespace Qydha.Domain.Services.Implementation;

public class InfluencerCodesService(IInfluencerCodesRepo influencerCodesRepo, IPurchaseService purchaseService) : IInfluencerCodesService
{
    private readonly IInfluencerCodesRepo _influencerCodesRepo = influencerCodesRepo;
    private readonly IPurchaseService _purchaseService = purchaseService;

    public async Task<Result<InfluencerCode>> AddInfluencerCode(string code, int numOfDays, DateTime? expireDate)
    {
        var getCodeRes = await _influencerCodesRepo.IsCodeAvailable(code);
        return getCodeRes
        .OnSuccessAsync(async () => await _influencerCodesRepo.AddAsync(new InfluencerCode(code, numOfDays, expireDate)));
    }

    public async Task<Result<User>> UseInfluencerCode(Guid userId, string code)
    {
        var getCodeRes = await _influencerCodesRepo.GetByCodeAsync(code);
        return getCodeRes
            
        .OnSuccessAsync(async (influencerCode) =>
            await _purchaseService.SubscribeInFree(userId, influencerCode));
    }
}
