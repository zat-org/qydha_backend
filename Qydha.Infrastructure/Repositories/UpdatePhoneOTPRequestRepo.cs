namespace Qydha.Infrastructure.Repositories;

public class UpdatePhoneOTPRequestRepo(QydhaContext qydhaContext, ILogger<UpdatePhoneOTPRequestRepo> logger) : IUpdatePhoneOTPRequestRepo
{

    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UpdatePhoneOTPRequestRepo> _logger = logger;

    public async Task<Result<UpdatePhoneRequest>> AddAsync(UpdatePhoneRequest request)
    {
        if (!_dbCtx.Users.Any(u => u.Id == request.UserId))
            return Result.Fail(new EntityNotFoundError<Guid>(request.UserId, nameof(User)));
        await _dbCtx.UpdatePhoneRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<UpdatePhoneRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.UpdatePhoneRequests.FirstOrDefaultAsync(req => req.Id == requestId) is UpdatePhoneRequest req ?
            Result.Ok(req) :
            Result.Fail(new EntityNotFoundError<Guid>(requestId, nameof(UpdatePhoneRequest)));
    }
    public async Task<Result> MarkRequestAsUsed(Guid requestId)
    {
        var affected = await _dbCtx.UpdatePhoneRequests.Where(req => req.Id == requestId).ExecuteUpdateAsync(
               setters => setters
                   .SetProperty(req => req.UsedAt, DateTimeOffset.UtcNow)
           );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(requestId, nameof(UpdatePhoneRequest)));
    }
}
