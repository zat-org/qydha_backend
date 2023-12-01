namespace Qydha.Infrastructure.Repositories;

public class UserRepo : IUserRepo
{
    private readonly IDbConnection _dbConnection;
    public UserRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "Users";
    private HashSet<string> DbAddExcludedProperties { get; } = new() { "Id" };
    private HashSet<string> DbUpdateExcludedProperties { get; } = new() { "Id" };
    private bool CheckPropIsExcluded(string propName, DbAction dbAction)
    {
        return dbAction switch
        {
            DbAction.Add => !DbAddExcludedProperties.Contains(propName),
            DbAction.Update => !DbUpdateExcludedProperties.Contains(propName),
            _ => false,
        };
    }
    private string DbColumns(DbAction dbAction)
    {
        return string.Join(", ", typeof(User).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => p.Name));
    }
    private string DbValues(DbAction dbAction)
    {
        return string.Join(", ", typeof(User).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => $"@{p.Name}"));
    }
    private string DbColumnsValuesForPUTUpdate()
    {
        return string.Join(", ", typeof(User).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, DbAction.Update))
                .Select(p => $"{p.Name} = @{p.Name}"));
    }

    #region AddUser
    public async Task<Result<User>> AddAsync(User user)
    {
        var sql = @$"INSERT INTO  {TableName} ({DbColumns(DbAction.Add)})  VALUES ( {DbValues(DbAction.Add)}) RETURNING Id;";
        var userId = await _dbConnection.QuerySingleAsync<Guid>(sql, user);
        user.Id = userId;
        return Result.Ok(user);
    }
    #endregion

    #region DeleteUser
    public async Task<Result> DeleteByIdAsync(Guid id)
    {
        var sql = @$"DELETE FROM {TableName} WHERE id = @Id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "User not Found" });
    }
    #endregion

    #region getUser
    public async Task<Result<IEnumerable<User>>> GetAsync(int pageSize = 10, int pageNumber = 1, UserType? userType = null)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@limit", pageSize, DbType.Int32);
        parameters.Add("@offset", (pageNumber - 1) * pageSize, DbType.Int32);

        var condStr = string.Empty;
        if (userType.HasValue)
        {
            condStr = "WHERE is_anonymous = @is_anonymous";
            parameters.Add("@is_anonymous", userType == UserType.Anonymous, DbType.Boolean);
        }
        var sql = @$"SELECT * FROM {TableName} 
                    {condStr}
                    ORDER BY created_on DESC
                    LIMIT @limit OFFSET @offset ;";

        IEnumerable<User> users = await _dbConnection.QueryAsync<User>(sql, parameters);
        return Result.Ok(users);
    }
    public async Task<Result<IEnumerable<User>>> GetAsync(Func<User, bool> criteriaFunc)
    {
        var parameters = new DynamicParameters();

        var sql = @$"SELECT * FROM {TableName} ;";

        IEnumerable<User> users = await _dbConnection.QueryAsync<User>(sql, parameters);

        return Result.Ok(users.Where(criteriaFunc));
    }
    private async Task<Result<User>> GetByPropAsync<T>(string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add($"@{propName}", propValue);
        var sql = $"SELECT *  from {TableName} where {propName} = @{propName};";
        User? user = await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, parameters);
        if (user is null)
            return Result.Fail<User>(new Error() { Code = ErrorCodes.UserNotFound, Message = "User Not Found" });
        return Result.Ok(user);
    }
    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        return await GetByPropAsync("Id", id);
    }
    public async Task<Result<User>> GetByPhoneAsync(string phone)
    {
        return await GetByPropAsync("phone", phone);
    }
    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        return await GetByPropAsync("Normalized_Email", email.ToUpper());
    }
    public async Task<Result<User>> GetByUsernameAsync(string username)
    {
        return await GetByPropAsync("Normalized_Username", username.ToUpper());
    }

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error { Code = ErrorCodes.DbUniqueViolation, Message = "اسم المستخدم موجود بالفعل" });
        return Result.Ok();
    }

    public async Task<Result> IsPhoneAvailable(string phone)
    {
        Result<User> getUserRes = await GetByPhoneAsync(phone);
        if (getUserRes.IsSuccess)
            return Result.Fail(new Error { Code = ErrorCodes.DbUniqueViolation, Message = "رقم الجوال موجود بالفعل" });
        return Result.Ok();
    }

    public async Task<Result> IsEmailAvailable(string email, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByEmailAsync(email);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error { Code = ErrorCodes.DbUniqueViolation, Message = "البريد الالكتروني موجود بالفعل." });
        return Result.Ok();
    }

    #endregion

    #region editUser
    public async Task<Result<User>> PutByIdAsync(User user)
    {
        var sql = @$"UPDATE {TableName} 
                    SET {DbColumnsValuesForPUTUpdate()}
                    WHERE id = @id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, user);
        return effectedRows == 1 ?
            Result.Ok(user) :
            Result.Fail<User>(new Error { Code = ErrorCodes.UserNotFound, Message = "User not Found" });
    }
    public async Task<Result> PatchById(Guid id, Dictionary<string, object> props)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        var propsNamesInQueryList = new List<string>();
        foreach (var prop in props)
        {
            parameters.Add($"@{prop.Key}", prop.Value);
            propsNamesInQueryList.Add($"{prop.Key} = @{prop.Key}");
        }
        var sql = @$"UPDATE {TableName} 
                     SET {string.Join(",", propsNamesInQueryList)} 
                     WHERE id = @id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
           Result.Ok() :
           Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "User not Found" });
    }
    public async Task<Result> PatchById<T>(Guid id, string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add($"@{propName}", propValue);
        var sql = @$"UPDATE {TableName} 
                     SET {propName} = @{propName}
                     WHERE id = @id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "User not Found" });
    }

    public async Task<Result> UpdateUserLastLoginToNow(Guid userId) =>
               await PatchById(userId, "last_login", DateTime.UtcNow);

    public async Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken) =>
            await PatchById(userId, "FCM_Token", fcmToken);

    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash) =>
                await PatchById(userId, "password_hash", passwordHash);
    public async Task<Result> UpdateUserUsername(Guid userId, string username) =>
                    await PatchById(userId, new() {
                        { "username", username },
                        { "Normalized_Username", username.ToUpper() }
                    });
    public async Task<Result> UpdateUserPhone(Guid userId, string phone) =>
                   await PatchById(userId, "phone", phone);

    public async Task<Result> UpdateUserEmail(Guid userId, string email) =>
                     await PatchById(userId, new() {
                        { "email", email },
                        { "Normalized_Email", email.ToUpper() },
                        { "is_email_confirmed", true }
                     });

    public async Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl) =>
    await PatchById(userId, new() {
                        { "Avatar_Path", avatarPath },
                        { "Avatar_Url", avatarUrl }
    });
    #endregion


    public async Task<Result<User>> CheckUserCredentials(Guid userId, string password)
    {
        Result<User> getUserRes = await GetByIdAsync(userId);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password_Hash))
                return Result.Fail<User>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> CheckUserCredentials(string username, string password)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password_Hash))
                return Result.Fail<User>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }
}