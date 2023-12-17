namespace Qydha.Infrastructure.Repositories;
public class RegistrationOTPRequestRepo(IDbConnection dbConnection, ILogger<RegistrationOTPRequestRepo> logger) : GenericRepository<RegistrationOTPRequest>(dbConnection, logger), IRegistrationOTPRequestRepo
{

}
