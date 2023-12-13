namespace Qydha.Domain.Services.Contracts;

public interface IAdminUserService
{
    Task<Result<Tuple<AdminUser, string>>> Login(string username, string password);
    Task<Result<AdminUser>> ChangePassword(Guid adminUserId, string oldPassword, string newPassword);
}
