
namespace Qydha.Infrastructure.Repositories;

public class UserBalootSettingsRepo(QydhaContext dbContext, ILogger<UserBalootSettingsRepo> logger) : IUserBalootSettingsRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<UserBalootSettingsRepo> _logger = logger;

    public async Task<Result<UserBalootSettings>> GetByUserIdAsync(Guid userId)
    {
        return await _dbCtx.UserBalootSettings.FirstOrDefaultAsync((settings) => settings.UserId == userId) is UserBalootSettings settings ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(UserBalootSettings)));

    }

    public async Task<Result<UserBalootSettings>> UpdateByUserIdAsync(UserBalootSettings settings)
    {
        var affected = await _dbCtx.UserBalootSettings.Where(settingsElm => settingsElm.UserId == settings.UserId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(settingsRow => settingsRow.IsFlipped, settings.IsFlipped)
                .SetProperty(settingsRow => settingsRow.IsAdvancedRecording, settings.IsAdvancedRecording)
                .SetProperty(settingsRow => settingsRow.IsSakkahMashdodahMode, settings.IsSakkahMashdodahMode)
                .SetProperty(settingsRow => settingsRow.ShowWhoWonDialogOnDraw, settings.ShowWhoWonDialogOnDraw)
                .SetProperty(settingsRow => settingsRow.IsNumbersSoundEnabled, settings.IsNumbersSoundEnabled)
                .SetProperty(settingsRow => settingsRow.IsCommentsSoundEnabled, settings.IsCommentsSoundEnabled)
        );
        return affected == 1 ?
            Result.Ok(settings) :
            Result.Fail(new EntityNotFoundError<Guid>(settings.UserId, nameof(UserBalootSettings)));
    }
}
