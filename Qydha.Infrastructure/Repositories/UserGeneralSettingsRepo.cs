

namespace Qydha.Infrastructure.Repositories;

public class UserGeneralSettingsRepo(QydhaContext dbContext, ILogger<UserGeneralSettingsRepo> logger) : IUserGeneralSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserGeneralSettingsRepo> _logger = logger;

    public async Task<Result<UserGeneralSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.UserGeneralSettings.FirstOrDefaultAsync((settings) => settings.UserId == userId) is UserGeneralSettings settings ?
          Result.Ok(settings) :
          Result.Fail<UserGeneralSettings>(new()
          {
              Code = ErrorType.UserGeneralSettingsNotFound,
              Message = $"User General Settings NotFound:: Entity not found"
          });
    }

    public async Task<Result<UserGeneralSettings>> UpdateByUserIdAsync(UserGeneralSettings settings)
    {
        var affected = await _dbCtx.UserGeneralSettings.Where(settingsElm => settingsElm.UserId == settings.UserId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(settingsRow => settingsRow.EnableVibration, settings.EnableVibration)
               .SetProperty(settingsRow => settingsRow.PlayersNames, settings.PlayersNames)
               .SetProperty(settingsRow => settingsRow.TeamsNames, settings.TeamsNames)
       );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail<UserGeneralSettings>(new()
            {
                Code = ErrorType.UserGeneralSettingsNotFound,
                Message = $"User General Settings NotFound:: Entity not found"
            });
    }
}
