namespace Qydha.Domain.Repositories;

public interface IAdminUserRepo //: IGenericRepository<AdminUser>
{
    Task<Result<AdminUser>> GetByIdAsync(Guid id);
    Task<Result<AdminUser>> GetByUsernameAsync(string username);
    Task<Result> IsUsernameAvailable(string username, Guid? userId = null);
    Task<Result> UpdateUserPassword(Guid userId, string passwordHash);
    Task<Result> UpdateUserUsername(Guid userId, string username);
    Task<Result<AdminUser>> CheckUserCredentials(Guid userId, string password);
    Task<Result<AdminUser>> CheckUserCredentials(string username, string password);
}

