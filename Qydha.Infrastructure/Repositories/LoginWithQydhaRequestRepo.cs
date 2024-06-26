﻿

namespace Qydha.Infrastructure.Repositories;

public class LoginWithQydhaRequestRepo(QydhaContext qydhaContext, ILogger<LoginWithQydhaRequestRepo> logger) : ILoginWithQydhaRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<LoginWithQydhaRequestRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.LoginWithQydhaRequestNotFound,
        Message = "Login With Qydha Request NotFound :: Entity Not Found"
    };
    public async Task<Result<LoginWithQydhaRequest>> AddAsync(LoginWithQydhaRequest request)
    {
        await _dbCtx.LoginWithQydhaRequests.AddAsync(request);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(request);
    }

    public async Task<Result<LoginWithQydhaRequest>> GetByIdAsync(Guid requestId)
    {
        return await _dbCtx.LoginWithQydhaRequests.FirstOrDefaultAsync(req => req.Id == requestId) is LoginWithQydhaRequest req ?
               Result.Ok(req) :
               Result.Fail<LoginWithQydhaRequest>(NotFoundError);
    }

    public async Task<Result> MarkRequestAsUsed(Guid requestId)
    {
        var affected = await _dbCtx.LoginWithQydhaRequests.Where(req => req.Id == requestId).ExecuteUpdateAsync(
               setters => setters
                   .SetProperty(req => req.UsedAt, DateTimeOffset.UtcNow)
           );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }

}
