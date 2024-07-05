namespace Qydha.Infrastructure.Repositories;

public class UserRepo(QydhaContext qydhaContext, ILogger<UserRepo> logger) : IUserRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<UserRepo> _logger = logger;

    #region  add User
    public async Task<Result<User>> AddAsync(User user)
    {
        await _dbCtx.Users.AddAsync(user);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(user);
    }
    #endregion

    #region getUser
    public async Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId)
    {
        return await _dbCtx.Users
            // .Include(user => user.UserGeneralSettings)
            // .Include(user => user.UserBalootSettings)
            // .Include(user => user.UserHandSettings)
            .AsSplitQuery()
            .FirstOrDefaultAsync((user) => user.Id == userId) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

    public async Task<Result<IEnumerable<User>>> GetAllRegularUsers()
    {
        var users = await _dbCtx.Users.ToListAsync();
        return Result.Ok((IEnumerable<User>)users);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.Id == id) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<Guid>(id, nameof(User)));

    public async Task<Result<User>> GetByPhoneAsync(string phone) =>
       await _dbCtx.Users.FirstOrDefaultAsync((user) => user.Phone == phone) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<string>(phone, nameof(User)));

    public async Task<Result<User>> GetByEmailAsync(string email) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.NormalizedEmail == email.ToUpper()) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<string>(email, nameof(User)));

    public async Task<Result<User>> GetByUsernameAsync(string username) =>
        await _dbCtx.Users.FirstOrDefaultAsync((user) => user.NormalizedUsername == username.ToUpper()) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<string>(username, nameof(User)));

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new EntityUniqueViolationError("newUsername", "اسم المستخدم موجود بالفعل"));
        return Result.Ok();
    }

    public async Task<Result> IsPhoneAvailable(string phone)
    {
        Result<User> getUserRes = await GetByPhoneAsync(phone);
        if (getUserRes.IsSuccess)
            return Result.Fail(new EntityUniqueViolationError("newPhone", "رقم الجوال موجود بالفعل"));
        return Result.Ok();
    }

    public async Task<Result> IsEmailAvailable(string email, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByEmailAsync(email);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new EntityUniqueViolationError("newEmail", "البريد الاكترونى موجود بالفعل"));
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
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.FCMToken, fcmToken)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.PasswordHash, passwordHash)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> UpdateUserUsername(Guid userId, string username)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Username, username)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> UpdateUserPhone(Guid userId, string phone)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Phone, phone)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> UpdateUserEmail(Guid userId, string email)
    {
        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.Email, email)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
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
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result<User>> UpdateUserExpireDate(Guid userId)
    {
        var allTransactions = await _dbCtx.UserPromoCodes
            .Where(p => p.UsedAt != null && p.UserId == userId)
            .Select(p => new Transaction() { NumberOfDays = p.NumberOfDays, UsedAt = p.UsedAt!.Value })
            .Union(_dbCtx.InfluencerCodeUserLinks
            .Where(l => l.UserId == userId)
            .Select(p => new Transaction() { NumberOfDays = p.NumberOfDays, UsedAt = p.UsedAt }))
            .Union(_dbCtx.Purchases
            .Where(p => p.UserId == userId)
            .Select(p => new Transaction() { NumberOfDays = p.NumberOfDays, UsedAt = p.PurchaseDate }))
            .OrderBy(t => t.UsedAt)
            .ToListAsync();

        DateTimeOffset? expireAt = null;
        allTransactions.ForEach((transaction) =>
        {
            if (expireAt is not null && transaction.UsedAt <= expireAt)
                expireAt = expireAt.Value.AddDays(transaction.NumberOfDays);
            else
                expireAt = transaction.UsedAt.AddDays(transaction.NumberOfDays);
        });

        var affected = await _dbCtx.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(user => user.ExpireDate, expireAt)
        );
        if (affected != 1)
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
        return await GetUserWithSettingsByIdAsync(userId);
    }

    #endregion
    public async Task<Result<User>> CheckUserCredentials(Guid userId, string password)
    {
        var checkRes = (await GetByIdAsync(userId))
               .OnSuccess((user) =>
               {
                   if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                       return Result.Fail(new InvalidCredentialsError("Invalid credentials"));
                   return Result.Ok(user);
               });
        if (checkRes.IsFailed)
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        else
            return checkRes;
    }
    public async Task<Result<User>> CheckUserCredentials(string username, string password)
    {
        var checkRes = (await GetByUsernameAsync(username))
            .OnSuccess((user) =>
               {
                   if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                       return Result.Fail(new InvalidCredentialsError("Invalid credentials"));
                   return Result.Ok(user);
               });
        if (checkRes.IsFailed)
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        else
            return checkRes;
    }
    public async Task<Result<User>> UpdateAsync(User user)
    {
        _dbCtx.Update(user);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(user);
    }
    public async Task<Result> DeleteAsync(Guid userId)
    {
        var affected = await _dbCtx.Users.Where(c => c.Id == userId).ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

}
internal class Transaction
{
    public DateTimeOffset UsedAt { get; set; }
    public int NumberOfDays { get; set; }
}