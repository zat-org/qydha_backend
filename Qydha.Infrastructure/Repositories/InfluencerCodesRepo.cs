namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesRepo(IDbConnection dbConnection, ILogger<InfluencerCodesRepo> logger) : GenericRepository<InfluencerCode>(dbConnection, logger), IInfluencerCodesRepo
{
    public async Task<Result<InfluencerCode>> GetByIdAsync(Guid id) =>
        await GetByUniquePropAsync(nameof(InfluencerCode.Id), id);

    public async Task<Result<InfluencerCode>> GetByCodeAsync(string code) =>
        await GetByUniquePropAsync(nameof(InfluencerCode.NormalizedCode), code.ToUpper());

    public async Task<Result> IsCodeAvailable(string code)
    {
        Result<InfluencerCode> getCodeRes = await GetByCodeAsync(code);
        if (getCodeRes.IsSuccess)
            return Result.Fail(new()
            {
                Code = ErrorCodes.DbUniqueViolation,
                Message = "هذا الكود موجود بالفعل"
            });
        return Result.Ok();
    }

    #region editInfluencerCode

    public async Task<Result> UpdateCode(Guid codeId, string code) =>
                    await PatchById(codeId, new Dictionary<string, object>() {
                       { nameof(InfluencerCode.Code),code},
                        {nameof(InfluencerCode.NormalizedCode) ,code.ToUpper() }
                    });

    public async Task<Result> UpdateExpireDate(Guid codeId, DateTime expireAt) =>
                await PatchById(codeId, nameof(InfluencerCode.ExpireAt), expireAt);

    public async Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays) =>
                await PatchById(codeId, nameof(InfluencerCode.NumberOfDays), numOfDays);

    #endregion

}
