namespace Qydha.Infrastructure.Repositories;

public class InfluencerCodesRepo(QydhaContext qydhaContext, ILogger<InfluencerCodesRepo> logger) : IInfluencerCodesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<InfluencerCodesRepo> _logger = logger;

    #region Get Code Data
    public async Task<Result<InfluencerCode>> GetByIdAsync(Guid id)
    {
        return await _dbCtx.InfluencerCodes
            .Include(c => c.Category)
            .FirstOrDefaultAsync(code => code.Id == id) is InfluencerCode code ?
                Result.Ok(code) :
                Result.Fail(new EntityNotFoundError<Guid>(id, nameof(InfluencerCode)));
    }
    public async Task<Result<int>> GetUserUsageCountByIdAsync(Guid userId, Guid codeId) =>
         Result.Ok(await _dbCtx.InfluencerCodeUserLinks
            .Where(link => link.UserId == userId && link.InfluencerCodeId == codeId)
            .CountAsync());
    public async Task<Result<int>> GetUsersUsageCountByIdAsync(Guid codeId) =>
             Result.Ok(await _dbCtx.InfluencerCodeUserLinks
                .Where(link => link.InfluencerCodeId == codeId)
                .CountAsync());
    public async Task<Result<InfluencerCode>> GetByCodeAsync(string codeName)
    {
        return await _dbCtx.InfluencerCodes
            .Include(c => c.Category)
            .FirstOrDefaultAsync(code => code.NormalizedCode == codeName.ToUpper()) is InfluencerCode influencerCode ?
           Result.Ok(influencerCode) :
           Result.Fail(new EntityNotFoundError<string>(codeName, nameof(InfluencerCode)));
    }
    public async Task<Result<InfluencerCode>> GetByCodeIfValidAsync(string codeName)
    {
        var code = await _dbCtx.InfluencerCodes
            .Include(c => c.Category)
            .FirstOrDefaultAsync(code => code.NormalizedCode == codeName.ToUpper());
        if (code == null)
            return Result.Fail(new EntityNotFoundError<string>(codeName, nameof(InfluencerCode)));
        if (code.ExpireAt is not null && code.ExpireAt.Value < DateTimeOffset.UtcNow)
            return Result.Fail(new InfluencerCodeExpiredError(code.ExpireAt.Value));
        return Result.Ok(code);
    }

    public async Task<Result> IsCodeAvailable(string code)
    {
        Result<InfluencerCode> getCodeRes = await GetByCodeAsync(code);
        if (getCodeRes.IsSuccess)
            return Result.Fail(new EntityUniqueViolationError("اسم الكود موجود بالفعل"));
        return Result.Ok();
    }

    #endregion

    #region editInfluencerCode

    public async Task<Result> UpdateCode(Guid codeId, string code)
    {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c => c.Code, code)
                .SetProperty(c => c.NormalizedCode, code.ToUpper())
        );
        return effected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(codeId, nameof(InfluencerCode)));
    }

    public async Task<Result> UpdateExpireDate(Guid codeId, DateTimeOffset expireAt)
    {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c => c.ExpireAt, expireAt)
        );
        return effected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(codeId, nameof(InfluencerCode)));
    }

    public async Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays)
    {
        var effected = await _dbCtx.InfluencerCodes.Where(c => c.Id == codeId).ExecuteUpdateAsync(
            setter => setter
                .SetProperty(c => c.NumberOfDays, numOfDays)
        );
        return effected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(codeId, nameof(InfluencerCode)));
    }

    #endregion

    #region add code or links
    public async Task<Result<InfluencerCode>> AddAsync(InfluencerCode code)
    {
        await _dbCtx.InfluencerCodes.AddAsync(code);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(code);
    }

    public async Task<Result<InfluencerCode>> UseInfluencerCode(Guid userId, InfluencerCode code)
    {
        var link = new InfluencerCodeUserLink()
        {
            UserId = userId,
            InfluencerCodeId = code.Id,
            UsedAt = DateTimeOffset.UtcNow,
            NumberOfDays = code.NumberOfDays
        };
        await _dbCtx.InfluencerCodeUserLinks.AddAsync(link);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(code);
    }
    #endregion
}
