
namespace Qydha.Infrastructure.Repositories;

public class UserPromoCodesRepo(QydhaContext qydhaContext, ILogger<UserPromoCodesRepo> logger) : IUserPromoCodesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UserPromoCodesRepo> _logger = logger;

    public async Task<Result<UserPromoCode>> AddAsync(UserPromoCode code)
    {
        if (!_dbCtx.Users.Any(u => u.Id == code.UserId))
            return Result.Fail(new EntityNotFoundError<Guid>(code.UserId, nameof(User)));
        await _dbCtx.UserPromoCodes.AddAsync(code);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(code);
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetUserValidPromoCodeAsync(Guid userId)
    {
        var codes = await _dbCtx.UserPromoCodes
            .Where(code => code.UserId == userId && code.UsedAt == null && code.ExpireAt >= DateTimeOffset.UtcNow)
            .OrderBy(code => code.ExpireAt)
            .ToListAsync();
        return Result.Ok((IEnumerable<UserPromoCode>)codes);
    }

    public async Task<Result<UserPromoCode>> GetByIdAsync(Guid codeId)
    {
        return await _dbCtx.UserPromoCodes.FirstOrDefaultAsync(code => code.Id == codeId) is UserPromoCode code ?
                Result.Ok(code) :
                Result.Fail<UserPromoCode>(new EntityNotFoundError<Guid>(codeId, nameof(UserPromoCode)));
    }

    public async Task<Result> MarkAsUsedByIdAsync(Guid codeId)
    {
        var affected = await _dbCtx.UserPromoCodes.Where(code => code.Id == codeId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(code => code.UsedAt, DateTimeOffset.UtcNow)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(codeId, nameof(UserPromoCode)));
    }
}