

namespace Qydha.Infrastructure.Repositories;

public class UserHandSettingsRepo(QydhaContext dbContext, ILogger<UserHandSettingsRepo> logger) : IUserHandSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserHandSettingsRepo> _logger = logger;

    public async Task<Result<UserHandSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.Users.FirstOrDefaultAsync((u) => u.Id == userId) is User user ?
            Result.Ok(user.UserHandSettings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserHandSettings)));
    }

    public async Task<Result<UserHandSettings>> UpdateByUserIdAsync(Guid userId, UserHandSettings settings)
    {
        var affected = await _dbCtx.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(u => u.UserHandSettings, settings)
       );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserHandSettings)));

    }
}
