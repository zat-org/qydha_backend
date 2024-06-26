﻿
namespace Qydha.Infrastructure.Repositories;

public class UpdateEmailRequestRepo(QydhaContext qydhaContext, ILogger<UpdateEmailRequestRepo> logger) : IUpdateEmailRequestRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UpdateEmailRequestRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.UpdateEmailRequestNotFound,
        Message = "Update Email Request NotFound :: Entity Not Found"
    };
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
                Result.Fail<UpdateEmailRequest>(NotFoundError);
    }
    public async Task<Result> MarkRequestAsUsed(Guid requestId)
    {
        var affected = await _dbCtx.UpdateEmailRequests.Where(req => req.Id == requestId).ExecuteUpdateAsync(
               setters => setters
                   .SetProperty(req => req.UsedAt, DateTimeOffset.UtcNow)
           );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
}
