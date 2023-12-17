namespace Qydha.Infrastructure.Repositories;

public class AdminUserRepo(IDbConnection dbConnection, ILogger<AdminUserRepo> logger) : GenericRepository<AdminUser>(dbConnection, logger), IAdminUserRepo
{

    #region getUser

    public async Task<Result<AdminUser>> GetByIdAsync(Guid id) =>
        await GetByUniquePropAsync(nameof(AdminUser.Id), id);

    public async Task<Result<AdminUser>> GetByUsernameAsync(string username) =>
        await GetByUniquePropAsync(nameof(AdminUser.NormalizedUsername), username.ToUpper());

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<AdminUser> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new()
            {
                Code = ErrorCodes.DbUniqueViolation,
                Message = "اسم المستخدم موجود بالفعل"
            });
        return Result.Ok();
    }

    #endregion

    #region editUser

    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash) =>
                await PatchById(userId,
                            nameof(AdminUser.PasswordHash),
                            passwordHash);

    public async Task<Result> UpdateUserUsername(Guid userId, string username) =>
                await PatchById(userId,
                    new Dictionary<string, object>()
                    {
                        {nameof(AdminUser.Username) ,username},
                        {nameof(AdminUser.NormalizedUsername) , username.ToUpper()}
                    });

    #endregion


    public async Task<Result<AdminUser>> CheckUserCredentials(Guid userId, string password)
    {
        Result<AdminUser> getUserRes = await GetByIdAsync(userId);
        return getUserRes.OnSuccess<AdminUser>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<AdminUser>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }

    public async Task<Result<AdminUser>> CheckUserCredentials(string username, string password)
    {
        Result<AdminUser> getUserRes = await GetByUsernameAsync(username);
        return getUserRes.OnSuccess<AdminUser>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<AdminUser>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }

}
