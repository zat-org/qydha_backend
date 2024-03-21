namespace Qydha.Infrastructure.Repositories;
public class PhoneAuthenticationRequestRepo(QydhaContext qydhaContext, ILogger<PhoneAuthenticationRequestRepo> logger) : IPhoneAuthenticationRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<PhoneAuthenticationRequestRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.PhoneAuthenticationRequestNotFound,
        Message = "Phone Authentication Request NotFound :: Entity Not Found"
    };
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
                Result.Fail<PhoneAuthenticationRequest>(NotFoundError);
    }

    public async Task<Result> MarkRequestAsUsed(Guid requestId)
    {
        var affected = await _dbCtx.PhoneAuthenticationRequests.Where(req => req.Id == requestId).ExecuteUpdateAsync(
               setters => setters
                   .SetProperty(req => req.UsedAt, DateTime.UtcNow)
           );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
}
