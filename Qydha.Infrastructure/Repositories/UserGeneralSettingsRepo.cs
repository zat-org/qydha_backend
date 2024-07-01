

namespace Qydha.Infrastructure.Repositories;

public class UserGeneralSettingsRepo(QydhaContext dbContext, ILogger<UserGeneralSettingsRepo> logger) : IUserGeneralSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserGeneralSettingsRepo> _logger = logger;

    public async Task<Result<UserGeneralSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.Users.FirstOrDefaultAsync((u) => u.Id == userId) is User user ?
            Result.Ok(user.UserGeneralSettings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

    public async Task<Result<UserGeneralSettings>> UpdateByUserIdAsync(Guid userId, UserGeneralSettings settings)
    {
        var affected = await _dbCtx.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(u => u.UserGeneralSettings, settings)
        );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserGeneralSettings)));
    }
}
