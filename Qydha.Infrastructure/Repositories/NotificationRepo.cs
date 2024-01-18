
namespace Qydha.Infrastructure.Repositories;
public class NotificationRepo(IDbConnection dbConnection, ILogger<NotificationRepo> logger) : GenericRepository<NotificationData>(dbConnection, logger), INotificationRepo
{
    public async Task<Result<int>> AssignNotificationToUser(Guid userId, int notificationId)
    {
        string sql = @$"INSERT INTO Notifications_Users_Link 
                            (Notification_Id ,User_Id ,Read_At ,Sent_At)
                        VALUES 
                            (@notificationId ,@userId ,NULL , NOW() )";
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

    public async Task<Result<int>> AssignNotificationToUser(Guid userId, NotificationData notification)
    {
        var notificationDataTableTuple = NotificationData.GetInsertQueryData(notification, excludeKey: true);
        var parameters = new DynamicParameters(notificationDataTableTuple.Item3);
        parameters.Add("@userId", userId);
        string query = @$"
            WITH inserted_notification AS (
                INSERT INTO {NotificationData.GetTableName()}  ({notificationDataTableTuple.Item1}) 
                VALUES ({notificationDataTableTuple.Item2}) 
                RETURNING {NotificationData.GetKeyColumnName()})
            
            INSERT INTO Notifications_Users_Link
                (Notification_Id, User_Id, Read_At, Sent_At)
            SELECT
                inserted_notification.id, @userId, NULL, NOW()  FROM inserted_notification;";
        try
        {
            int effectedRows = await _dbConnection.ExecuteAsync(query, parameters);
            return Result.Ok(effectedRows);

        }
        catch (DbException exp)
        {
            if (IsForeignKeyConstraintViolation(exp))
            {
                _logger.LogWarning("Foreign Key Constrain Violation in {sql} at userId = {userId} ", query, userId);
                return Result.Fail<int>(new Error()
                {
                    Code = ErrorType.DbForeignKeyViolation,
                    Message = $"Foreign Key Constrain Violation : {exp.Message} "
                });
            }
            else
            {
                _logger.LogCritical("Db Error at {sql} with error message : {msg} ", query, exp.Message);
                throw;
            }
        }
    }

    public async Task<Result<int>> AssignNotificationToAllUsers(NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Public;
        var notificationDataTableTuple = NotificationData.GetInsertQueryData(notification, excludeKey: true);
        var parameters = new DynamicParameters(notificationDataTableTuple.Item3);
        string query = @$"
            WITH inserted_notification AS (
                INSERT INTO {NotificationData.GetTableName()}  ({notificationDataTableTuple.Item1}) 
                VALUES ({notificationDataTableTuple.Item2}) 
                RETURNING {NotificationData.GetKeyColumnName()})
            
            INSERT INTO Notifications_Users_Link
                (Notification_Id, User_Id, Read_At, Sent_At)    
            SELECT
                inserted_notification.id, users.id , NULL, NOW()  FROM inserted_notification join users on users.is_Anonymous = false;";

        try
        {
            int effectedRows = await _dbConnection.ExecuteAsync(query, parameters);
            return Result.Ok(effectedRows);
        }
        catch (DbException exp)
        {
            _logger.LogCritical("Db Error at {sql} with error message : {msg} ", query, exp.Message);
            throw;
        }
    }

    public async Task<Result<NotificationData>> AssignNotificationToAllAnonymousUsers(NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Anonymous;
        return await AddAsync<int>(notification);
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@userId", userId);
        dynamicParameters.Add("@Limit", pageSize);
        dynamicParameters.Add("@Offset", (pageNumber - 1) * pageSize);

        string isReadCondition = isRead switch
        {
            null => "",
            true => $"AND nul.read_at is not null",
            false => $"AND nul.read_at is null"
        };

        string query = @$"
            SELECT nul.id , nd.title , nd.description , nd.action_path as ActionPath , nd.action_type as ActionType , nd.payload as payloadStr , nul.user_id as UserId  , nul.read_at as ReadAt , nul.sent_at as SentAt
            FROM notifications_data nd JOIN notifications_users_link nul ON nul.notification_id = nd.id
            WHERE nul.user_id = @userId {isReadCondition}
            ORDER BY nul.sent_at DESC
            LIMIT @Limit OFFSET @Offset  ;";

        try
        {
            var notifications = await _dbConnection.QueryAsync<Notification>(query, dynamicParameters);
            return Result.Ok(notifications);
        }
        catch (DbException exp)
        {
            _logger.LogCritical("Db Error at {sql} with error message : {msg} ", query, exp.Message);
            throw;
        }
    }

    public async Task<Result<IEnumerable<NotificationData>>> GetAllAnonymousUserNotification(int pageSize = 10, int pageNumber = 1)
    {
        NotificationVisibility[] visibilities = [NotificationVisibility.Public, NotificationVisibility.Anonymous];
        return (await GetAllAsync(
                    @$"{NotificationData.GetColumnName(nameof(NotificationData.Visibility))} IN  @Visibilities",
                    new { Visibilities = visibilities },
                    pageSize,
                    pageNumber,
                    $" {NotificationData.GetColumnName(nameof(NotificationData.CreatedAt))} Desc"))
                .OnSuccess<IEnumerable<NotificationData>>((notifications) =>
                {
                    notifications = notifications.OrderByDescending(n => n.CreatedAt);
                    return Result.Ok(notifications);
                });
    }

    public async Task<Result<int>> DeleteAllByUserIdAsync(Guid userId)
    {
        try
        {
            string query = $"DELETE FROM notifications_users_link WHERE user_id = @userId ;";
            _logger.LogTrace("Before Execute Query :: {query}", query);
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { userId });
            return Result.Ok(effectedRows);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg} ", exp.Message);
            throw;
        }
    }

    public async Task<Result<int>> DeleteNotificationByUserIdAsync(Guid userId, int notificationId)
    {
        try
        {
            string query = $"DELETE FROM notifications_users_link WHERE user_id = @userId AND id = @notificationId ;";
            _logger.LogTrace("Before Execute Query :: {query}", query);
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { userId, notificationId });
            return effectedRows == 1 ?
                Result.Ok(effectedRows) :
                Result.Fail<int>(new()
                {
                    Code = ErrorType.NotificationNotFound,
                    Message = $"NotificationNotFound :: Entity not found"
                });
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg}", [exp.Message]);
            throw;
        }

    }

    public async Task<Result<int>> MarkAllAsReadByUserIdAsync(Guid userId)
    {
        try
        {
            string query = $"UPDATE notifications_users_link SET read_at = NOW()  WHERE user_id = @Id ;";
            _logger.LogTrace("Before Execute Query :: {query}", query);
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { Id = userId });
            return Result.Ok(effectedRows);
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg} ", exp.Message);
            throw;
        }
    }

    public async Task<Result<int>> MarkNotificationAsReadByUserIdAsync(Guid userId, int notificationId)
    {
        try
        {
            string query = $"UPDATE notifications_users_link SET read_at = NOW()  WHERE user_id = @userId AND id = @notificationId ;";
            _logger.LogTrace("Before Execute Query :: {query}", query);
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { userId, notificationId });
            return effectedRows == 1 ?
                Result.Ok(effectedRows) :
                Result.Fail<int>(new()
                {
                    Code = ErrorType.NotificationNotFound,
                    Message = $"NotificationNotFound :: Entity not found"
                });
        }
        catch (DbException exp)
        {
            _logger.LogCritical(exp, "Error from db : {msg}", [exp.Message]);
            throw;
        }

    }

    public async Task<Result> ApplyAnonymousClickOnNotification(int notificationId)
    {
        var anonymousClicksColumnName = NotificationData.GetColumnName(nameof(NotificationData.AnonymousClicks));
        NotificationVisibility[] visibilities = [NotificationVisibility.Public, NotificationVisibility.Anonymous];
        string query = @$"
            UPDATE {NotificationData.GetTableName()} 
            SET {anonymousClicksColumnName} = {anonymousClicksColumnName} + 1 
            WHERE {NotificationData.GetKeyColumnName()} = @Id AND 
                {NotificationData.GetColumnName(nameof(NotificationData.Visibility))} IN  @Visibilities ;";
        try
        {
            int effectedRows = await _dbConnection.ExecuteAsync(query, new { Id = notificationId, Visibilities = visibilities });
            if (effectedRows == 1)
                return Result.Ok(effectedRows);
            else
                return Result.Fail(new()
                {
                    Code = ErrorType.NotificationNotFound,
                    Message = "Notification Not Found"
                });
        }
        catch (DbException exp)
        {
            _logger.LogCritical("Db Error at {sql} with error message : {msg} ", query, exp.Message);
            throw;
        }
    }
}