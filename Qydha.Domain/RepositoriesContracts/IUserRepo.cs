namespace Qydha.Domain.Repositories;
public interface IUserRepo
{
    Task<Result<User>> AddAsync(User user);
    Task<Result<User>> UpdateAsync(User user);
    Task<Result> DeleteAsync(Guid userId);
    Task<Result<User>> GetByIdForDashboardAsync(Guid userId);
    Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId);
    Task<Result<PagedList<User>>> GetAllRegularUsers(PaginationParameters parameters, UsersFilterParameters filterParameters);
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByPhoneAsync(string phone);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> GetByUsernameAsync(string username);
    Task<Result> IsUsernameAvailable(string username, Guid? userId = null);
    Task<Result> IsPhoneAvailable(string phone);
    Task<Result> IsEmailAvailable(string email, Guid? userId = null);
    Task<Result> IsUserSubscribed(Guid userId);
    Task<Result> UpdateUserLastLoginToNow(Guid userId);
    Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken);
    Task<Result> UpdateUserPassword(Guid userId, string passwordHash);
    Task<Result> UpdateUserUsername(Guid userId, string username);
    Task<Result> UpdateUserPhone(Guid userId, string phone);
    Task<Result> UpdateUserEmail(Guid userId, string email);
    Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl);
    Task<Result<User>> UpdateUserExpireDate(Guid userId);
    Task<Result<User>> CheckUserCredentials(Guid userId, string password);
    Task<Result<User>> CheckUserCredentials(string username, string password);
}
