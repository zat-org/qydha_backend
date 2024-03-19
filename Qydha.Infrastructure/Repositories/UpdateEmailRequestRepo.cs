
namespace Qydha.Infrastructure.Repositories;

public class UpdateEmailRequestRepo(QydhaContext qydhaContext, ILogger<UpdateEmailRequestRepo> logger) : IUpdateEmailRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UpdateEmailRequestRepo> _logger = logger;
    public async Task<Result<UpdateEmailRequest>> AddAsync(UpdateEmailRequest request)
    {
        await _dbCtx.UpdateEmailRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<UpdateEmailRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.UpdateEmailRequests.FirstOrDefaultAsync(req => req.Id == requestId) is UpdateEmailRequest req ?
                Result.Ok(req) :
                Result.Fail<UpdateEmailRequest>(new()
                {
                    Code = ErrorType.UpdateEmailRequestNotFound,
                    Message = "Update Email Request NotFound :: Entity Not Found"
                });
    }
}
