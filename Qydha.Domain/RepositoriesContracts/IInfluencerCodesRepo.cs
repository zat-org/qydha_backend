namespace Qydha.Domain.Repositories;

public interface IInfluencerCodesRepo
{
    Task<Result<InfluencerCode>> GetByIdAsync(Guid id);
    Task<Result<InfluencerCode>> GetByCodeAsync(string code);
    Task<Result> IsCodeAvailable(string code);
    Task<Result> UpdateCode(Guid codeId, string code);
    Task<Result> UpdateExpireDate(Guid codeId, DateTimeOffset expireAt);
    Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays);
    Task<Result<InfluencerCode>> GetByCodeIfValidAsync(string codeName);
    Task<Result<int>> GetUserUsageCountByIdAsync(Guid userId, Guid codeId);
    Task<Result<int>> GetUsersUsageCountByIdAsync(Guid codeId);
    Task<Result<InfluencerCode>> AddAsync(InfluencerCode code);
    Task<Result<InfluencerCode>> UseInfluencerCode(Guid userId, InfluencerCode code);
}

