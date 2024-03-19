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
                Result.Fail<RegistrationOTPRequest>(new()
                {
                    Code = ErrorType.RegistrationOTPRequestNotFound,
                    Message = "Registration OTP Request NotFound :: Entity Not Found"
                });
    }
}
