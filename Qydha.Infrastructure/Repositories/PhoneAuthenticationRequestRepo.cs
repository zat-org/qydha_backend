
namespace Qydha.Infrastructure.Repositories;

public class PhoneAuthenticationRequestRepo(IDbConnection dbConnection, ILogger<PhoneAuthenticationRequestRepo> logger) : GenericRepository<PhoneAuthenticationRequest>(dbConnection, logger), IPhoneAuthenticationRequestRepo
{


}
