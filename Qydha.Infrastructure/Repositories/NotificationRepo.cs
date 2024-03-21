
namespace Qydha.Infrastructure.Repositories;
public class NotificationRepo(QydhaContext qydhaContext, ILogger<NotificationRepo> logger) : INotificationRepo
{
    private readonly QydhaContext _dbCtx = qydhaContext;
    private readonly ILogger<NotificationRepo> _logger = logger;
    private readonly Error NotFoundError = new()
    {
        Code = ErrorType.NotificationNotFound,
        Message = $"Notifications NotFound:: Entity not found"
    };
    public async Task<Result<NotificationData>> GetNotificationDataByIdAsync(int notificationDataId)
    {
        return await _dbCtx.NotificationsData.FirstOrDefaultAsync(n => n.Id == notificationDataId) is NotificationData notification ?
               Result.Ok(notification) :
               Result.Fail<NotificationData>(NotFoundError);
    }


    public async Task<Result<int>> AssignNotificationToUser(Guid userId, int notificationId)
    {
        var notificationLink = new NotificationUserLink()
        {
            UserId = userId,
            NotificationId = notificationId,
            ReadAt = null,
            //! utc
            SentAt = DateTime.Now
        };
        await _dbCtx.NotificationUserLinks.AddAsync(notificationLink);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(1);
    }

    public async Task<Result<int>> AssignNotificationToUser(Guid userId, NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Private;
        var notificationLink = new NotificationUserLink()
        {
            UserId = userId,
            ReadAt = null,
            //! utc
            SentAt = DateTime.Now
        };
        await _dbCtx.NotificationsData.AddAsync(notification);
        notification.NotificationUserLinks.Add(notificationLink);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(notification.NotificationUserLinks.Count);
    }

    public async Task<Result<int>> AssignNotificationToAllUsers(NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Public;
        await _dbCtx.NotificationsData.AddAsync(notification);
        await _dbCtx.Users.Where(u => !u.IsAnonymous).ForEachAsync((u) =>
        {
            notification.NotificationUserLinks.Add(new()
            {
                UserId = u.Id,
                ReadAt = null,
                //! utc
                SentAt = DateTime.Now
            });
        });
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(notification.NotificationUserLinks.Count);
    }

    public async Task<Result<NotificationData>> AssignNotificationToAllAnonymousUsers(NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Anonymous;
        await _dbCtx.NotificationsData.AddAsync(notification);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(notification);
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null)
    {
        //! TODO remove isRead OR apply it 
        List<Notification> notifications = await _dbCtx.NotificationUserLinks.Where(ul => ul.UserId == userId)
            .Include(ul => ul.Notification)
            .Select(ul => new Notification()
            {
                Id = ul.Id,
                Title = ul.Notification.Title,
                Description = ul.Notification.Description,
                ActionPath = ul.Notification.ActionPath,
                ActionType = ul.Notification.ActionType,
                Payload = ul.Notification.Payload,
                UserId = ul.UserId,
                ReadAt = ul.ReadAt,
                SentAt = ul.SentAt
            })
            .OrderByDescending(ul => ul.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Result.Ok((IEnumerable<Notification>)notifications);
    }

    public async Task<Result<IEnumerable<NotificationData>>> GetAllAnonymousUserNotification(int pageSize = 10, int pageNumber = 1)
    {
        NotificationVisibility[] visibilities = [NotificationVisibility.Public, NotificationVisibility.Anonymous];
        List<NotificationData> notifications = await _dbCtx.NotificationsData.Where(n => visibilities.Contains(n.Visibility))
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Result.Ok((IEnumerable<NotificationData>)notifications);
    }

    public async Task<Result<int>> DeleteAllByUserIdAsync(Guid userId)
    {
        //! TODO Check IF notification deleted only from the link not from notifications Data => delete data that not connected to any user
        var affected = await _dbCtx.NotificationUserLinks.Where(ul => ul.UserId == userId).ExecuteDeleteAsync();
        return affected > 0 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> DeleteNotificationByUserIdAsync(Guid userId, int notificationId)
    {
        //! TODO Check IF notification deleted only from the link not from notifications Data => delete data that not connected to any user
        var affected = await _dbCtx.NotificationUserLinks
                .Where(ul => ul.UserId == userId && ul.Id == notificationId)
                .ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> MarkAllAsReadByUserIdAsync(Guid userId)
    {
        // ! TODO UTC
        var affected = await _dbCtx.NotificationUserLinks.Where(ul => ul.UserId == userId).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(ul => ul.ReadAt, DateTime.Now)
       );
        return affected > 0 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> MarkNotificationAsReadByUserIdAsync(Guid userId, int notificationId)
    {
        // ! TODO UTC
        var affected = await _dbCtx.NotificationUserLinks
            .Where(ul => ul.UserId == userId && ul.Id == notificationId)
            .ExecuteUpdateAsync(
            setters => setters
                .SetProperty(ul => ul.ReadAt, DateTime.Now)
            );
        return affected == 1 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result> ApplyAnonymousClickOnNotification(int notificationId)
    {
        var affected = await _dbCtx.NotificationsData
            .Where(n => n.Id == notificationId)
            .ExecuteUpdateAsync(
            setters => setters
                .SetProperty(ul => ul.AnonymousClicks, ul => ul.AnonymousClicks + 1)
            );
        return affected == 1 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }
}