

namespace Qydha.Infrastructure.Repositories;

public class UserHandSettingsRepo(QydhaContext dbContext, ILogger<UserHandSettingsRepo> logger) : IUserHandSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserHandSettingsRepo> _logger = logger;

    public async Task<Result<UserHandSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.UserHandSettings.FirstOrDefaultAsync((settings) => settings.UserId == userId) is UserHandSettings settings ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserHandSettings)));
    }

    public async Task<Result<UserHandSettings>> UpdateByUserIdAsync(UserHandSettings settings)
    {
        var affected = await _dbCtx.UserHandSettings.Where(settingsElm => settingsElm.UserId == settings.UserId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(settingsRow => settingsRow.RoundsCount, settings.RoundsCount)
               .SetProperty(settingsRow => settingsRow.MaxLimit, settings.MaxLimit)
               .SetProperty(settingsRow => settingsRow.TeamsCount, settings.TeamsCount)
               .SetProperty(settingsRow => settingsRow.PlayersCountInTeam, settings.PlayersCountInTeam)
               .SetProperty(settingsRow => settingsRow.WinUsingZat, settings.WinUsingZat)
               .SetProperty(settingsRow => settingsRow.TakweeshPoints, settings.TakweeshPoints)
       );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(settings.UserId, nameof(UserHandSettings)));

    }
}
