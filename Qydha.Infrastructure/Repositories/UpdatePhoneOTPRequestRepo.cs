namespace Qydha.Infrastructure.Repositories;

public class UpdatePhoneOTPRequestRepo(IDbConnection dbConnection, ILogger<UpdatePhoneOTPRequestRepo> logger) : GenericRepository<UpdatePhoneRequest>(dbConnection, logger), IUpdatePhoneOTPRequestRepo
{



}
