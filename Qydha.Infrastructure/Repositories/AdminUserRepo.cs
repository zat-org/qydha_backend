namespace Qydha.Infrastructure.Repositories;

public class AdminUserRepo(IDbConnection dbConnection) : IAdminUserRepo
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private const string TableName = "Admins";
    private HashSet<string> DbAddExcludedProperties { get; } = ["Id"];
    private HashSet<string> DbUpdateExcludedProperties { get; } = ["Id"];
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
        return string.Join(", ", typeof(AdminUser).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => p.Name));
    }
    private string DbValues(DbAction dbAction)
    {
        return string.Join(", ", typeof(AdminUser).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, dbAction))
                .Select(p => $"@{p.Name}"));
    }
    private string DbColumnsValuesForPUTUpdate()
    {
        return string.Join(", ", typeof(AdminUser).GetProperties()
                .Where((p) => CheckPropIsExcluded(p.Name, DbAction.Update))
                .Select(p => $"{p.Name} = @{p.Name}"));
    }
    #region AddAdminUser
    public async Task<Result<AdminUser>> AddAsync(AdminUser adminUser)
    {
        var sql = @$"INSERT INTO  {TableName} ({DbColumns(DbAction.Add)})  VALUES ( {DbValues(DbAction.Add)}) RETURNING Id;";
        var id = await _dbConnection.QuerySingleAsync<Guid>(sql, adminUser);
        adminUser.Id = id;
        return Result.Ok(adminUser);
    }
    #endregion

    #region DeleteUser
    public async Task<Result> DeleteByIdAsync(Guid id)
    {
        var sql = @$"DELETE FROM {TableName} WHERE id = @Id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
        return effectedRows == 1 ?
          Result.Ok() :
          Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "Admin User not Found" });
    }
    #endregion

    #region getUser

    public async Task<Result<IEnumerable<AdminUser>>> GetAsync(Func<AdminUser, bool>? criteriaFunc = null)
    {
        var parameters = new DynamicParameters();

        var sql = @$"SELECT * FROM {TableName} ;";

        IEnumerable<AdminUser> admins = await _dbConnection.QueryAsync<AdminUser>(sql, parameters);

        return criteriaFunc is null ? Result.Ok(admins) : Result.Ok(admins.Where(criteriaFunc));
    }
    private async Task<Result<AdminUser>> GetByPropAsync<T>(string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add($"@{propName}", propValue);
        var sql = $"SELECT *  from {TableName} where {propName} = @{propName};";
        AdminUser? adminUser = await _dbConnection.QuerySingleOrDefaultAsync<AdminUser>(sql, parameters);
        if (adminUser is null)
            return Result.Fail<AdminUser>(new Error() { Code = ErrorCodes.UserNotFound, Message = "Admin User Not Found" });
        return Result.Ok(adminUser);
    }
    public async Task<Result<AdminUser>> GetByIdAsync(Guid id)
    {
        return await GetByPropAsync("Id", id);
    }

    public async Task<Result<AdminUser>> GetByUsernameAsync(string username)
    {
        return await GetByPropAsync("Normalized_Username", username.ToUpper());
    }

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<AdminUser> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error { Code = ErrorCodes.DbUniqueViolation, Message = "اسم المستخدم موجود بالفعل" });
        return Result.Ok();
    }

    #endregion

    #region editUser
    public async Task<Result<AdminUser>> PutByIdAsync(AdminUser adminUser)
    {
        var sql = @$"UPDATE {TableName} 
                    SET {DbColumnsValuesForPUTUpdate()}
                    WHERE id = @id;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, adminUser);
        return effectedRows == 1 ?
            Result.Ok(adminUser) :
            Result.Fail<AdminUser>(new Error { Code = ErrorCodes.UserNotFound, Message = "Admin User not Found" });
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
           Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "Admin User not Found" });
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
          Result.Fail(new Error { Code = ErrorCodes.UserNotFound, Message = "Admin User not Found" });
    }

    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash) =>
                await PatchById(userId, "password_hash", passwordHash);
    public async Task<Result> UpdateUserUsername(Guid userId, string username) =>
                    await PatchById(userId, new() {
                        { "username", username },
                        { "Normalized_Username", username.ToUpper() }
                    });
    #endregion


    public async Task<Result<AdminUser>> CheckUserCredentials(Guid userId, string password)
    {
        Result<AdminUser> getUserRes = await GetByIdAsync(userId);
        return getUserRes.OnSuccess<AdminUser>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password_Hash))
                return Result.Fail<AdminUser>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }

    public async Task<Result<AdminUser>> CheckUserCredentials(string username, string password)
    {
        Result<AdminUser> getUserRes = await GetByUsernameAsync(username);
        return getUserRes.OnSuccess<AdminUser>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password_Hash))
                return Result.Fail<AdminUser>(new() { Code = ErrorCodes.InvalidCredentials, Message = "incorrect password" });
            return Result.Ok(user);
        });
    }

}
