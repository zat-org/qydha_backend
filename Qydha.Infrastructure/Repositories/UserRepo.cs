

namespace Qydha.Infrastructure.Repositories;

public class UserRepo(QydhaContext qydhaContext, ILogger<UserRepo> logger) : IUserRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UserRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.UserNotFound,
        Message = $"User Not Found :: Entity not found"
    };

    #region  add User
    public async Task<Result<User>> AddAsync(User user)
    {
        await _dbCtx.Users.AddAsync(user);
        //! TODO check tracking here
        user.UserGeneralSettings = new UserGeneralSettings();
        user.UserBalootSettings = new UserBalootSettings();
        user.UserHandSettings = new UserHandSettings();
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(user);
    }
    #endregion

    #region getUser
    public async Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId)
    {
        return await _dbCtx.Users.Include(user => user.UserGeneralSettings)
            .Include(user => user.UserBalootSettings)
            .Include(user => user.UserHandSettings)
            .AsSplitQuery()
            .FirstOrDefaultAsync((user) => user.Id == userId) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);
    }

    public async Task<Result<IEnumerable<User>>> GetAllRegularUsers()
    {
        var users = await _dbCtx.Users.Where(user => user.IsAnonymous == false).ToListAsync();
        return Result.Ok((IEnumerable<User>)users);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.Id == id) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);

    public async Task<Result<User>> GetByPhoneAsync(string phone) =>
       await _dbCtx.Users.FirstOrDefaultAsync((user) => user.Phone == phone) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);

    public async Task<Result<User>> GetByEmailAsync(string email) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.NormalizedEmail == email.ToUpper()) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);

    public async Task<Result<User>> GetByUsernameAsync(string username) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.NormalizedUsername == username.ToUpper()) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);

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
    public async Task<Result> UpdateUserLastLoginToNow(Guid userId)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.LastLogin, DateTimeOffset.UtcNow)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.FCMToken, fcmToken)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.PasswordHash, passwordHash)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserUsername(Guid userId, string username)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Username, username)
                .SetProperty(user => user.NormalizedEmail, username.ToUpper())
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserPhone(Guid userId, string phone)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Phone, phone)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserEmail(Guid userId, string email)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Email, email)
                .SetProperty(user => user.NormalizedEmail, email.ToUpper())
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }
    public async Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.AvatarPath, avatarPath)
                .SetProperty(user => user.AvatarUrl, avatarUrl)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(NotFoundError);
    }


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
                    Message = "كلمة المرور غير صحيحة"
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
                    Message = "كلمة المرور غير صحيحة"
                });
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> UpdateAsync(User user)
    {
        var affected = await _dbCtx.Users.Where(userRaw => userRaw.Id == user.Id).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(userRaw => userRaw.Name, user.Name)
               .SetProperty(userRaw => userRaw.BirthDate, user.BirthDate)
       );
        return affected == 1 ?
            Result.Ok(user) :
            Result.Fail<User>(NotFoundError);
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var affected = await _dbCtx.Users.Where(c => c.Id == userId).ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new Error()
            {
                Code = ErrorType.UserNotFound,
                Message = "User Not Found :: Entity Not Found"
            });
    }
}