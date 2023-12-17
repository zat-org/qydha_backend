namespace Qydha.Infrastructure.Repositories;

public class UpdateEmailRequestRepo(IDbConnection dbConnection, ILogger<UpdateEmailRequestRepo> logger) : GenericRepository<UpdateEmailRequest>(dbConnection, logger), IUpdateEmailRequestRepo
{

}
