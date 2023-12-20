
namespace Qydha.Infrastructure.Repositories;

public class UserGeneralSettingsRepo(IDbConnection dbConnection, ILogger<UserGeneralSettingsRepo> logger) : GenericRepository<UserGeneralSettings>(dbConnection, logger),  IUserGeneralSettingsRepo
{
    // public override Task<Result<UserGeneralSettings>> GetByUniquePropAsync<IdT>(string propName, IdT propValue)
    // {
    //     return base.GetByUniquePropAsync(propName, propValue);
    // }
}
