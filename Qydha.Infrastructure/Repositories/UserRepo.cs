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
    public async Task<Result<User>> GetByIdForDashboardAsync(Guid userId)
    {
        return await _dbCtx.Users
                .Include(u => u.UserPromoCodes)
                .Include(u => u.Purchases)
                .Include(u => u.InfluencerCodes).ThenInclude(c => c.InfluencerCode).ThenInclude(c => c.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync((user) => user.Id == userId) is User user ?
                Result.Ok(user) :
                Result.Fail<User>(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result> IsUserSubscribed(Guid userId)
    {
        return (await _dbCtx.Users
            .AnyAsync((user) => user.Id == userId && user.ExpireDate >= DateTimeOffset.UtcNow)) ?
                Result.Ok() :
                Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

    public async Task<Result> IsUserStreamer(Guid userId)
    {
        return (await _dbCtx.Users
                   .AnyAsync((user) => user.Id == userId && user.Roles.Contains(UserRoles.Streamer))) ?
                       Result.Ok() :
                       Result.Fail(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

    public async Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId)
    {
        return await _dbCtx.Users
            .AsSplitQuery()
            .FirstOrDefaultAsync((user) => user.Id == userId) is User user ?
            Result.Ok(user) :
            Result.Fail<User>(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }
    public async Task<Result<BalootGameBoard>> GetBalootBoardByUserIdAsync(Guid userId)
    {
        return await _dbCtx.Users.AsSplitQuery()
            .FirstOrDefaultAsync((user) => user.Id == userId && user.Roles.Contains(UserRoles.Streamer)) is User user ?
            Result.Ok(user.BalootGameBoards) :
            Result.Fail<BalootGameBoard>(new EntityNotFoundError<Guid>(userId, nameof(User)));
    }

    public async Task<Result<PagedList<User>>> GetAllRegularUsers(PaginationParameters parameters, UsersFilterParameters filterParameters)
    {
        IQueryable<User> query = _dbCtx.Users;
        if (filterParameters.Role != null)
            query = query.Where(u => u.Roles.Contains(filterParameters.Role.Value));
        if (!string.IsNullOrEmpty(filterParameters.SearchToken))
        {
            string token = filterParameters.SearchToken.ToLower().Trim();
            query = query.Where(u =>
                EF.Functions.Like(u.Username.ToLower(), $"%{token}%") ||
                (u.Email != null && EF.Functions.Like(u.Email.ToLower(), $"%{token}%")) ||
                EF.Functions.Like(u.Phone.ToLower(), $"%{token}%") ||
                EF.Functions.Like(u.Id.ToString().ToLower(), $"%{token}%")
            );
        }
        query = query.OrderByDescending(g => g.CreatedAt);
        PagedList<User> users = await _dbCtx.GetPagedData(query, parameters.PageNumber, parameters.PageSize);
        return Result.Ok(users);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id) =>
        await _dbCtx.Users
            .FirstOrDefaultAsync((user) => user.Id == id) is User user ?
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

    public async Task<Result> IsUsernameAndPhoneAvailable(string username, string phone)
    {
        return await _dbCtx.Users.AnyAsync(u => u.Username == username || u.Phone == phone) ?
            Result.Fail(new EntityUniqueViolationError(nameof(username), " اسم المستخدم او رقم الجوال مستخدم بالفعل")) :
            Result.Ok();
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
            .Where(p => p.UserId == userId && p.RefundedAt == null)
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
        var user = await _dbCtx.Users
            .FirstOrDefaultAsync((user) => user.NormalizedUsername == username.ToUpper());
        if (user == null)
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        else
            return Result.Ok(user);
    }
    public async Task<Result<User>> UpdateAsync(User userWithNewData)
    {
        var trackedUser = await _dbCtx.Users
           .AsSplitQuery()
           .AsTracking(QueryTrackingBehavior.TrackAll)
           .FirstOrDefaultAsync((u) => u.Id == userWithNewData.Id);
        if (trackedUser == null)
            return Result.Fail<User>(new EntityNotFoundError<Guid>(userWithNewData.Id, nameof(User)));
        trackedUser.UpdateData(userWithNewData);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(userWithNewData);
    }
    public async Task<Result> DeleteAsync(User user)
    {
        _dbCtx.Entry(user).State = EntityState.Deleted;
        await _dbCtx.SaveChangesAsync();
        return Result.Ok();
    }


}
internal class Transaction
{
    public DateTimeOffset UsedAt { get; set; }
    public int NumberOfDays { get; set; }
}