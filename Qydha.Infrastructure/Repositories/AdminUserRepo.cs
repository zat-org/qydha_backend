
namespace Qydha.Infrastructure.Repositories;

public class AdminUserRepo(QydhaContext dbContext, ILogger<AdminUserRepo> logger) : IAdminUserRepo
{
    private readonly QydhaContext _dbCtx = dbContext;
    private readonly ILogger<AdminUserRepo> _logger = logger;


    #region getUser

    public async Task<Result<AdminUser>> GetByIdAsync(Guid id)
    {
        return await _dbCtx.Admins.FirstOrDefaultAsync((admin) => admin.Id == id) is AdminUser admin ?
            Result.Ok(admin) :
            Result.Fail(new EntityNotFoundError<Guid>(id, nameof(AdminUser)));
    }

    public async Task<Result<AdminUser>> GetByUsernameAsync(string username)
    {
        return await _dbCtx.Admins.FirstOrDefaultAsync((admin) => admin.NormalizedUsername == username.ToUpper()) is AdminUser admin ?
            Result.Ok(admin) :
            Result.Fail(new EntityNotFoundError<string>(username, nameof(AdminUser)));

    }

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<AdminUser> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new EntityUniqueViolationError("اسم المستخدم موجود بالفعل"));
        return Result.Ok();
    }

    #endregion

    #region editUser

    public async Task<Result> UpdateUserPassword(Guid adminId, string passwordHash)
    {
        var affected = await _dbCtx.Admins.Where(admin => admin.Id == adminId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(admin => admin.PasswordHash, passwordHash)
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(adminId, nameof(AdminUser)));

    }

    public async Task<Result> UpdateUserUsername(Guid adminId, string username)
    {
        var affected = await _dbCtx.Admins.Where(admin => admin.Id == adminId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(admin => admin.Username, username)
                .SetProperty(admin => admin.NormalizedUsername, username.ToUpper())
        );
        return affected == 1 ?
            Result.Ok() :
            Result.Fail(new EntityNotFoundError<Guid>(adminId, nameof(AdminUser)));

    }
    #endregion

    public async Task<Result<AdminUser>> CheckUserCredentials(Guid userId, string password)
    {
        var checkRes = (await GetByIdAsync(userId))
            .OnSuccess(adminUser => PasswordHashingManager.VerifyPassword(password, adminUser.PasswordHash));
        if (checkRes.IsFailed)
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        else
            return checkRes;
    }


    public async Task<Result<AdminUser>> CheckUserCredentials(string username, string password)
    {
        var checkRes = (await GetByUsernameAsync(username))
            .OnSuccess(adminUser => PasswordHashingManager.VerifyPassword(password, adminUser.PasswordHash));
        if (checkRes.IsFailed)
            return Result.Fail(new InvalidCredentialsError("اسم المستخدم او كلمة المرور غير صحيحة"));
        else
            return checkRes;
    }
}
