
namespace Qydha.Infrastructure.Repositories;

public class UpdatePhoneOTPRequestRepo(QydhaContext qydhaContext, ILogger<UpdatePhoneOTPRequestRepo> logger) : IUpdatePhoneOTPRequestRepo
{

    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UpdatePhoneOTPRequestRepo> _logger = logger;
    public async Task<Result<UpdatePhoneRequest>> AddAsync(UpdatePhoneRequest request)
    {
        await _dbCtx.UpdatePhoneRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<UpdatePhoneRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.UpdatePhoneRequests.FirstOrDefaultAsync(req => req.Id == requestId) is UpdatePhoneRequest req ?
                Result.Ok(req) :
                Result.Fail<UpdatePhoneRequest>(new()
                {
                    Code = ErrorType.UpdatePhoneRequestNotFound,
                    Message = "Update Phone Request NotFound :: Entity Not Found"
                });
    }
}
