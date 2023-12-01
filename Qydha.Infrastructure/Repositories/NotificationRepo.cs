namespace Qydha.Infrastructure.Repositories;
public class NotificationRepo : INotificationRepo
{
    private readonly IDbConnection _dbConnection;

    public NotificationRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    private const string TableName = "Notification";

    private string DbColumns(HashSet<string> ExcludedProps)
    {
        return string.Join(", ", typeof(Notification).GetProperties()
                .Where((p) => !ExcludedProps.Contains(p.Name))
                .Select(p => p.Name));
    }
    private string DbValues(HashSet<string> ExcludedProps)
    {
        return string.Join(", ", typeof(Notification).GetProperties()
                .Where((p) => !ExcludedProps.Contains(p.Name))
                .Select(p => $"@{p.Name}"));
    }

    public async Task<Result<Notification>> AddAsync(Notification notification)
    {
        HashSet<string> excludedProps = new() { "Notification_Id" };
        var sql = @$"INSERT INTO {TableName} ({DbColumns(excludedProps)}) VALUES ( {DbValues(excludedProps)}) RETURNING Notification_Id;";
        var notificationId = await _dbConnection.QuerySingleAsync<int>(sql, notification);
        notification.Notification_Id = notificationId;
        return Result.Ok(notification);
    }

    public async Task<Result> DeleteByIdAsync(Guid userId, int id)
    {
        var sql = @$"DELETE FROM {TableName} WHERE Notification_Id = @Id And User_Id = @userId;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { Id = id, userId });
        return effectedRows == 1 ?
            Result.Ok() :
            Result.Fail(new() { Code = ErrorCodes.NotificationNotFound, Message = "Notification Not Found" });
    }

    public async Task<Result<int>> DeleteAllByUserIdAsync(Guid userId)
    {
        var sql = @$"DELETE FROM {TableName} WHERE user_id = @UserId;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
        return Result.Ok(effectedRows);
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, Func<Notification, bool> filterCriteria, int pageSize = 10, int pageNumber = 1)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        parameters.Add("@limit", pageSize);
        parameters.Add("@offset", (pageNumber - 1) * pageSize);
        var sql = @$"
                    SELECT * FROM {TableName} 
                    WHERE user_id = @userId 
                    order by created_at desc 
                    LIMIT @limit OFFSET @offset ;
                    ";
        var notifications = await _dbConnection.QueryAsync<Notification>(sql, parameters);
        return Result.Ok(notifications.Where(filterCriteria));
    }

    public async Task<Result> PatchById<T>(Guid userId, int id, string propName, T propValue)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add("@userId", userId);
        parameters.Add($"@{propName}", propValue);
        var sql = @$"UPDATE {TableName} 
                     SET {propName} = @{propName}
                     WHERE Notification_Id = @id And User_Id = @userId ;";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return effectedRows == 1 ?
            Result.Ok() :
            Result.Fail(new() { Code = ErrorCodes.NotificationNotFound, Message = "Notification Not Found" });
    }

    public Task<Result> MarkNotificationAsRead(Guid userId, int notificationId)
    {
        return PatchById(userId, notificationId, "Read_At", DateTime.UtcNow);
    }

    public async Task<Result<int>> AddToUsersWithCriteria(Notification notification, string filteringCriteria = "")
    {
        filteringCriteria = string.IsNullOrEmpty(filteringCriteria) ? string.Empty : $" Where  {filteringCriteria}";
        var sql = @$"
        INSERT INTO {TableName} (User_Id, Title, Description, Read_At, Created_At, Action_Path, Action_Type)
            SELECT
            Users.Id , @Title, @Description, @Read_At, @Created_At, @Action_Path, @Action_Type
            FROM Users {filteringCriteria};";

        var effectedRows = await _dbConnection.ExecuteAsync(sql, notification);
        return Result.Ok(effectedRows);
    }

    public async Task<Result<int>> AddToUsersWithByIds(Notification notification, IEnumerable<Guid> ids)
    {
        var parameters = new DynamicParameters();
        HashSet<string> excludedProps = new() { "Notification_Id" };
        var columns = DbColumns(excludedProps);
        excludedProps.Add("User_id");
        var valuesArr = new List<string>();

        foreach (var prop in typeof(Notification).GetProperties()
                .Where((p) => !excludedProps.Contains(p.Name)))
        {
            parameters.Add($"@{prop.Name}", prop.GetValue(notification));
            valuesArr.Add($"@{prop.Name}");
        }

        var values = string.Join(" , ", valuesArr);
        parameters.Add("@Ids", ids);

        var sql = @$"
        INSERT INTO {TableName} ({columns})
            SELECT
            Users.Id ,
            {values}
            FROM Users WHERE users.Id IN (@Ids);";
        var effectedRows = await _dbConnection.ExecuteAsync(sql, notification);
        return Result.Ok(effectedRows);
    }


}

