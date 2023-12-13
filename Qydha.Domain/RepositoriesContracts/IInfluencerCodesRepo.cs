namespace Qydha.Domain.Repositories;

public interface IInfluencerCodesRepo
{
    Task<Result<InfluencerCode>> AddAsync(InfluencerCode influencerCode);
    Task<Result> DeleteByIdAsync(Guid id);
    Task<Result<IEnumerable<InfluencerCode>>> GetAsync(Func<InfluencerCode, bool>? criteriaFunc);
    Task<Result<InfluencerCode>> GetByIdAsync(Guid id);
    Task<Result<InfluencerCode>> GetByCodeAsync(string code);
    Task<Result> IsCodeAvailable(string code);
    Task<Result<InfluencerCode>> PutByIdAsync(InfluencerCode code);
    Task<Result> UpdateCode(Guid codeId, string code);
    Task<Result> UpdateExpireDate(Guid codeId, DateTime expireAt);
    Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays);
}

