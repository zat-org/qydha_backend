namespace Qydha.Domain.Repositories;
public interface IUserRepo
{
    Task<Result<User>> AddAsync(User user);
    Task<Result> DeleteByIdAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetAsync(int pageSize = 10, int pageNumber = 1, UserType? userType = null);
    Task<Result<IEnumerable<User>>> GetAsync(Func<User, bool> criteriaFunc);
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByPhoneAsync(string phone);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> GetByUsernameAsync(string username);
    Task<Result> IsUsernameAvailable(string username, Guid? userId = null);
    Task<Result> IsPhoneAvailable(string phone);
    Task<Result> IsEmailAvailable(string email, Guid? userId = null);

    Task<Result<User>> PutByIdAsync(User user);
    Task<Result> PatchById(Guid id, Dictionary<string, object> props);
    Task<Result> PatchById<T>(Guid id, string propName, T propValue);
    Task<Result> UpdateUserLastLoginToNow(Guid userId);
    Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken);
    Task<Result> UpdateUserPassword(Guid userId, string passwordHash);
    Task<Result> UpdateUserUsername(Guid userId, string username);
    Task<Result> UpdateUserPhone(Guid userId, string phone);
    Task<Result> UpdateUserEmail(Guid userId, string email);
    Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl);
    Task<Result<User>> CheckUserCredentials(Guid userId, string password);
    Task<Result<User>> CheckUserCredentials(string username, string password);
}
