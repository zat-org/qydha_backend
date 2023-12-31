namespace Qydha.Domain.Repositories;
public interface IUserRepo : IGenericRepository<User>
{
    Task<Result<Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>>> GetUserWithSettingsByIdAsync(Guid userId);
    Task<Result<IEnumerable<User>>> GetAllRegularUsers();
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByPhoneAsync(string phone);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> GetByUsernameAsync(string username);
    Task<Result> IsUsernameAvailable(string username, Guid? userId = null);
    Task<Result> IsPhoneAvailable(string phone);
    Task<Result> IsEmailAvailable(string email, Guid? userId = null);

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
