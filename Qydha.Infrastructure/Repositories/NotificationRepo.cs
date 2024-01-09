
namespace Qydha.Infrastructure.Repositories;
public class NotificationRepo(IDbConnection dbConnection, ILogger<NotificationRepo> logger) : GenericRepository<Notification>(dbConnection, logger), INotificationRepo
{
    public async Task<Result> DeleteByIdAndUserIdAsync(Guid userId, int notificationId)
    {
        string colName = Notification.GetColumnName(nameof(Notification.UserId));
        string criteria = $"{colName} = @userId";
        return await DeleteByIdAsync(notificationId, criteria, new { userId });
    }

    public async Task<Result<int>> DeleteAllByUserIdAsync(Guid userId)
    {
        try
        {
            var sql = @$"DELETE FROM {Notification.GetTableName()} WHERE user_id = @UserId;";
            var effectedRows = await _dbConnection.ExecuteAsync(sql, new { UserId = userId });
            return Result.Ok(effectedRows);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : #msg ", [exp.Message]);
            throw;
        }
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null)
    {
        string isReadCondition = isRead switch
        {
            null => "",
            true => $"AND {Notification.GetColumnName(nameof(Notification.ReadAt))} is not null",
            false => $"AND {Notification.GetColumnName(nameof(Notification.ReadAt))} is null"
        };
        return (await GetAllAsync(
                @$"{Notification.GetColumnName(nameof(Notification.UserId))} = @userId 
                    {isReadCondition} ",
                new { userId }, pageSize, pageNumber, $" {Notification.GetColumnName(nameof(Notification.CreatedAt))} Desc "))
                .OnSuccess<IEnumerable<Notification>>((notifications) =>
                {
                    notifications = notifications.OrderByDescending(n => n.CreatedAt);
                    return Result.Ok(notifications);
                });
    }

    public Task<Result> MarkNotificationAsRead(Guid userId, int notificationId)
    {
        string colName = Notification.GetColumnName(nameof(Notification.UserId));
        string criteria = $"{colName} = @userId";
        return PatchById(notificationId, nameof(Notification.ReadAt), DateTime.UtcNow, criteria, new { userId });
    }

    public async Task<Result<int>> AddToUsersWithCriteria(Notification notification, string filteringCriteria = "", object? filterParams = null)
    {
        var parameters = new DynamicParameters(notification);
        if (filterParams is not null)
            parameters.AddDynamicParams(filterParams);

        string criteria = string.IsNullOrWhiteSpace(filteringCriteria) ? "" : $" WHERE  {filteringCriteria}";
        var sql = @$"
        INSERT INTO {Notification.GetTableName()} (User_Id, Title, Description,  Created_At, Action_Path, Action_Type)
            SELECT Users.Id , @Title, @Description, @CreatedAt, @ActionPath, @ActionType
            FROM Users {criteria} ;";
        _logger.LogInformation(sql);
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return Result.Ok(effectedRows);
    }

    public async Task<Result<int>> AddToUsersWithByIds(Notification notification, IEnumerable<Guid> ids)
    {
        var parameters = new DynamicParameters(notification);
        parameters.Add("@Ids", ids);
        var sql = @$"
        INSERT INTO {Notification.GetTableName()} (User_Id, Title, Description, Created_At, Action_Path, Action_Type)
            SELECT Users.Id , @Title, @Description, @CreatedAt, @ActionPath, @ActionType
            FROM Users WHERE users.Id IN ( @Ids );";
        _logger.LogInformation(sql);
        var effectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
        return Result.Ok(effectedRows);
    }

    public async Task<Result> AssignNotificationToUser(Guid userId, int notificationId)
    {
        string sql = @$"INSERT INTO Notifications_Users_Link 
                            (Notification_Id ,User_Id ,Read_At ,Sent_At)
                        VALUES 
                            (@notificationId ,@userId ,NULL , NOW())";
        try
        {
            int effectedRows = await _dbConnection.ExecuteAsync(sql, new { userId, notificationId });
            return Result.Ok(effectedRows);
        }
        catch (DbException exp)
        {
            if (IsForeignKeyConstraintViolation(exp))
            {
                _logger.LogWarning("Foreign Key Constrain Violation in {sql} at userId = {userId} , notificationId = {notificationId}", sql, userId, notificationId);
                return Result.Fail<int>(new Error()
                {
                    Code = ErrorType.DbForeignKeyViolation,
                    Message = $"Foreign Key Constrain Violation : {exp.Message} "
                });
            }
            else
            {
                _logger.LogCritical("Db Error at {sql} with error message : {msg} ", sql, exp.Message);
                throw;
            }
        }
    }
}