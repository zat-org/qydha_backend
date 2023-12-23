
namespace Qydha.Infrastructure.Repositories;

public class UserRepo(IDbConnection dbConnection, ILogger<UserRepo> logger) : GenericRepository<User>(dbConnection, logger), IUserRepo
{
    #region getUser
    public async Task<Result<IEnumerable<User>>> GetAllRegularUsers() =>
         await GetAllAsync(filterCriteria: $"{GetColumnName(nameof(User.IsAnonymous))} = false",
                                parameters: new { },
                                orderCriteria: "");

    public async Task<Result<User>> GetByIdAsync(Guid id) =>
        await GetByUniquePropAsync(nameof(User.Id), id);

    public async Task<Result<User>> GetByPhoneAsync(string phone) =>
        await GetByUniquePropAsync(nameof(User.Phone), phone);

    public async Task<Result<User>> GetByEmailAsync(string email) =>
        await GetByUniquePropAsync(nameof(User.NormalizedEmail), email.ToUpper());

    public async Task<Result<User>> GetByUsernameAsync(string username) =>
        await GetByUniquePropAsync(nameof(User.NormalizedUsername), username.ToUpper());

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "اسم المستخدم موجود بالفعل"
            });
        return Result.Ok();
    }

    public async Task<Result> IsPhoneAvailable(string phone)
    {
        Result<User> getUserRes = await GetByPhoneAsync(phone);
        if (getUserRes.IsSuccess)
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "رقم الجوال موجود بالفعل"
            });
        return Result.Ok();
    }

    public async Task<Result> IsEmailAvailable(string email, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByEmailAsync(email);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "البريد الالكتروني موجود بالفعل."
            });
        return Result.Ok();
    }

    #endregion

    #region editUser    
    public async Task<Result> UpdateUserLastLoginToNow(Guid userId) =>
                await PatchById(userId,
                                nameof(User.LastLogin),
                                 DateTime.UtcNow);

    public async Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken) =>
                await PatchById(userId,
                                nameof(User.FCMToken),
                               fcmToken);

    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash) =>
                await PatchById(userId,
                                nameof(User.PasswordHash),
                                passwordHash);
    public async Task<Result> UpdateUserUsername(Guid userId, string username) =>
                await PatchById(userId,
                    new Dictionary<string, object>(){
                        { nameof(User.Username) ,username},
                        { nameof(User.NormalizedUsername),username.ToUpper()}
                    });
    public async Task<Result> UpdateUserPhone(Guid userId, string phone) =>
                   await PatchById(userId,
                                nameof(User.Phone),
                                phone);

    public async Task<Result> UpdateUserEmail(Guid userId, string email) =>
                await PatchById(userId,
                new Dictionary<string, object>(){
                        { nameof(User.Email),email},
                        { nameof(User.NormalizedEmail),email.ToUpper()},
                        { nameof(User.IsEmailConfirmed) ,true}
                    });


    public async Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl) =>
                await PatchById(userId,
                 new Dictionary<string, object>(){
                        { nameof(User.AvatarPath),avatarPath},
                        { nameof(User.AvatarUrl),avatarUrl}
                    });

    #endregion

    public async Task<Result<User>> CheckUserCredentials(Guid userId, string password)
    {
        Result<User> getUserRes = await GetByIdAsync(userId);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidCredentials,
                    Message = "incorrect password"
                });
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> CheckUserCredentials(string username, string password)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidCredentials,
                    Message = "incorrect password"
                });
            return Result.Ok(user);
        });
    }


}