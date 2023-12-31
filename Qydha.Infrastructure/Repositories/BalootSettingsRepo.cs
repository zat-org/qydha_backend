namespace Qydha.Infrastructure.Repositories;

public class UserBalootSettingsRepo(IDbConnection dbConnection, ILogger<UserBalootSettingsRepo> logger) : GenericRepository<UserBalootSettings>(dbConnection, logger), IUserBalootSettingsRepo
{

}
