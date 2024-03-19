namespace Qydha.Infrastructure.Repositories;
public class PhoneAuthenticationRequestRepo(QydhaContext qydhaContext, ILogger<PhoneAuthenticationRequestRepo> logger) : IPhoneAuthenticationRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<PhoneAuthenticationRequestRepo> _logger = logger;
    public async Task<Result<PhoneAuthenticationRequest>> AddAsync(PhoneAuthenticationRequest request)
    {
        await _dbCtx.PhoneAuthenticationRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<PhoneAuthenticationRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.PhoneAuthenticationRequests.FirstOrDefaultAsync(req => req.Id == requestId) is PhoneAuthenticationRequest req ?
                Result.Ok(req) :
                Result.Fail<PhoneAuthenticationRequest>(new()
                {
                    Code = ErrorType.PhoneAuthenticationRequestNotFound,
                    Message = "Phone Authentication Request NotFound :: Entity Not Found"
                });
    }

}
