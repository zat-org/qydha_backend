namespace Qydha.Domain.Repositories;

public interface IAdminUserRepo
{
    Task<Result<AdminUser>> AddAsync(AdminUser adminUser);
    Task<Result> DeleteByIdAsync(Guid id);
    Task<Result<IEnumerable<AdminUser>>> GetAsync(Func<AdminUser, bool>? criteriaFunc = null);
    Task<Result<AdminUser>> GetByIdAsync(Guid id);
    Task<Result<AdminUser>> GetByUsernameAsync(string username);
    Task<Result> IsUsernameAvailable(string username, Guid? userId = null);
    Task<Result<AdminUser>> PutByIdAsync(AdminUser adminUser);
    Task<Result> PatchById(Guid id, Dictionary<string, object> props);
    Task<Result> PatchById<T>(Guid id, string propName, T propValue);
    Task<Result> UpdateUserPassword(Guid userId, string passwordHash);
    Task<Result> UpdateUserUsername(Guid userId, string username);
    Task<Result<AdminUser>> CheckUserCredentials(Guid userId, string password);
    Task<Result<AdminUser>> CheckUserCredentials(string username, string password);
}

