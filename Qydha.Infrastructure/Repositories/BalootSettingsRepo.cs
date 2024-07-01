
namespace Qydha.Infrastructure.Repositories;

public class UserBalootSettingsRepo(QydhaContext dbContext, ILogger<UserBalootSettingsRepo> logger) : IUserBalootSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserBalootSettingsRepo> _logger = logger;
    public async Task<Result<UserBalootSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.Users.FirstOrDefaultAsync((u) => u.Id == userId) is User user ?
            Result.Ok(user.UserBalootSettings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserBalootSettings)));

    }

    public async Task<Result<UserBalootSettings>> UpdateByUserIdAsync(Guid userId, UserBalootSettings settings)
    {
        var affected = await _dbCtx.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(u => u.UserBalootSettings, settings)
        );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserBalootSettings)));
    }
}
