
namespace Qydha.Infrastructure.Repositories;

public class UserHandSettingsRepo(IDbConnection dbConnection, ILogger<UserHandSettingsRepo> logger) : GenericRepository<UserHandSettings>(dbConnection, logger), IUserHandSettingsRepo
{
    // public override Task<Result<UserGeneralSettings>> GetByUniquePropAsync<IdT>(string propName, IdT propValue)
    // {
    //     return base.GetByUniquePropAsync(propName, propValue);
    // }
}
