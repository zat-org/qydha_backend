
namespace Qydha.Infrastructure.Repositories;

public class UserPromoCodesRepo(QydhaContext qydhaContext, ILogger<UserPromoCodesRepo> logger) : IUserPromoCodesRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UserPromoCodesRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.UserPromoCodeNotFound,
        Message = "User Promo Code NotFound :: Entity Not Found"
    };

    public async Task<Result<UserPromoCode>> AddAsync(UserPromoCode code)
    {
        await _dbCtx.UserPromoCodes.AddAsync(code);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(code);
    }

    public async Task<Result<IEnumerable<UserPromoCode>>> GetAllUserValidPromoCodeAsync(Guid userId)
    {
        var codes = await _dbCtx.UserPromoCodes
            .Where(code => code.UserId == userId && code.UsedAt == null && code.ExpireAt >= DateTime.Now).ToListAsync();
        return Result.Ok((IEnumerable<UserPromoCode>)codes);
    }

    public async Task<Result<UserPromoCode>> GetByIdAsync(Guid codeId)
    {
        return await _dbCtx.UserPromoCodes.FirstOrDefaultAsync(code => code.Id == codeId) is UserPromoCode code ?
                Result.Ok(code) :
                Result.Fail<UserPromoCode>(NotFoundError);
    }

    public async Task<Result> MarkAsUsedByIdAsync(Guid codeId)
    {
        var affected = await _dbCtx.UserPromoCodes.Where(code => code.Id == codeId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(code => code.UsedAt, DateTime.Now)
        //! convert to utc
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
}