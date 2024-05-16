namespace Qydha.Domain.Services.Contracts;

public interface IAdminUserService
{
    Task<Result<(AdminUser adminUser, string jwtToken)>> Login(string username, string password);
    Task<Result<AdminUser>> ChangePassword(Guid adminUserId, string oldPassword, string newPassword);
}
