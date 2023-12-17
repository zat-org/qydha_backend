namespace Qydha.Domain.Repositories;

public interface IInfluencerCodesRepo : IGenericRepository<InfluencerCode>
{
    Task<Result<InfluencerCode>> GetByIdAsync(Guid id);
    Task<Result<InfluencerCode>> GetByCodeAsync(string code);
    Task<Result> IsCodeAvailable(string code);
    Task<Result> UpdateCode(Guid codeId, string code);
    Task<Result> UpdateExpireDate(Guid codeId, DateTime expireAt);
    Task<Result> UpdateNumberOfDays(Guid codeId, int numOfDays);
}

