namespace Qydha.Domain.Services.Contracts;

public interface IAdminUserService
{
    Task<Result<(User user, string jwtToken)>> Login(string username, string password);
}
