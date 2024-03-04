

namespace Qydha.Infrastructure.Repositories;

public class LoginWithQydhaRequestRepo(IDbConnection dbConnection, ILogger<LoginWithQydhaRequestRepo> logger) : GenericRepository<LoginWithQydhaRequest>(dbConnection, logger), ILoginWithQydhaRequestRepo
{
    public async Task<Result> MarkRequestAsUsed(Guid requestId) => await PatchById(requestId, nameof(LoginWithQydhaRequest.UsedAt), DateTime.UtcNow);

}
