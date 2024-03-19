namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesRepo(QydhaContext qydhaContext , ILogger<InfluencerCodesRepo> logger) : IInfluencerCodesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext ;
    private readonly ILogger<InfluencerCodesRepo> _logger = logger ;

    public async Task<Result<InfluencerCode>> GetByIdAsync(Guid id) {
        return  await _dbCtx.InfluencerCodes.FirstOrDefaultAsync(code => code.Id == id) is InfluencerCode code ?
            Result.Ok(code) :
            Result.Fail<InfluencerCode> (new() {
                Code = ErrorType.InfluencerCodeNotFound,
                Message = "Influencer Code NotFound :: Entity Not Found"
            });
    }

    public async Task<Result<InfluencerCode>> GetByCodeAsync(string codeName) {
         return  await _dbCtx.InfluencerCodes.FirstOrDefaultAsync(code => code.NormalizedCode == codeName.ToUpper()) is InfluencerCode influencerCode ?
            Result.Ok(influencerCode) :
            Result.Fail<InfluencerCode> (new() {
                Code = ErrorType.InfluencerCodeNotFound,
                Message = "Influencer Code NotFound :: Entity Not Found"
            });
    }

    public async Task<Result> IsCodeAvailable(string code)
    {
        Result<InfluencerCode> getCodeRes = await GetByCodeAsync(code);
        if (getCodeRes.IsSuccess)
            return Result.Fail(new()
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "هذا الكود موجود بالفعل"
            });
        return Result.Ok();
    }

    #region editInfluencerCode

    public async Task<Result> UpdateCode(Guid codeId, string code) {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c=> c.Code , code)
                .SetProperty(c=> c.NormalizedCode , code.ToUpper())
        );
                    //! TODO Handle Change the name in purchases 

        return effected == 1 ? 
            Result.Ok() :
            Result.Fail(new() {
                Code = ErrorType.InfluencerCodeNotFound,
                Message = "Influencer Code NotFound :: Entity Not Found"
            });
    }
               
    public async Task<Result> UpdateExpireDate(Guid codeId, DateTime expireAt) {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c=> c.ExpireAt , expireAt)
        );
        return effected == 1 ? 
            Result.Ok() :
            Result.Fail(new() {
                Code = ErrorType.InfluencerCodeNotFound,
                Message = "Influencer Code NotFound :: Entity Not Found"
            });
    }

    public async Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays) {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c=> c.NumberOfDays , numOfDays)
        );
        return effected == 1 ? 
            Result.Ok() :
            Result.Fail(new() {
                Code = ErrorType.InfluencerCodeNotFound,
                Message = "Influencer Code NotFound :: Entity Not Found"
            });
    }

    public async Task<Result<InfluencerCode>> AddAsync(InfluencerCode code)
    {
        await _dbCtx.InfluencerCodes.AddAsync(code);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(code);
    }
    #endregion

}
