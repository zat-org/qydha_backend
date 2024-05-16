namespace Qydha.Infrastructure.Repositories;
public class RegistrationOTPRequestRepo(QydhaContext qydhaContext, ILogger<RegistrationOTPRequestRepo> logger) : IRegistrationOTPRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<RegistrationOTPRequestRepo> _logger = logger;

    public async Task<Result<RegistrationOTPRequest>> AddAsync(RegistrationOTPRequest request)
    {
        await _dbCtx.RegistrationOtpRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<RegistrationOTPRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.RegistrationOtpRequests.FirstOrDefaultAsync(req => req.Id == requestId) is RegistrationOTPRequest req ?
            Result.Ok(req) :
            Result.Fail(new EntityNotFoundError<Guid>(requestId, nameof(RegistrationOTPRequest), ErrorType.RegistrationOTPRequestNotFound));
    }
    public async Task<Result> MarkRequestAsUsed(Guid requestId, Guid userId)
    {
        var affected = await _dbCtx.RegistrationOtpRequests.Where(req => req.Id == requestId).ExecuteUpdateAsync(
               setters => setters
                   .SetProperty(req => req.UsedAt, DateTimeOffset.UtcNow)
           );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(requestId, nameof(RegistrationOTPRequest), ErrorType.RegistrationOTPRequestNotFound));
    }
}
