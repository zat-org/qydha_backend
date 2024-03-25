
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

    private static Notification MapToNotification(NotificationData n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Description = n.Description,
        ActionPath = n.ActionPath,
        ActionType = n.ActionType,
        Payload = n.Payload,
        SentAt = n.CreatedAt,
        TemplateValues = [n.TemplateValues]
    };
    private static Notification MapToNotification(NotificationData n, Dictionary<string, string> templateValues, DateTimeOffset? sentAt = null, DateTimeOffset? readAt = null)
    => new()
    {
        Id = n.Id,
        Title = n.Title,
        Description = n.Description,
        ActionPath = n.ActionPath,
        ActionType = n.ActionType,
        Payload = n.Payload,
        SentAt = sentAt != null ? sentAt.Value : n.CreatedAt,
        ReadAt = readAt,
        TemplateValues = [n.TemplateValues, templateValues]
    };

    public async Task<Result<NotificationData>> GetByIdAsync(int notificationDataId)
    {
        return await _dbCtx.NotificationsData.FirstOrDefaultAsync(n => n.Id == notificationDataId) is NotificationData notification ?
               Result.Ok(notification) :
               Result.Fail<NotificationData>(NotFoundError);
    }

    public async Task<Result<Notification>> AssignToUser(Guid userId, int notificationId, Dictionary<string, string> templateValues)
    {
        return (await GetByIdAsync(notificationId))
        .OnSuccessAsync(async (notificationData) =>
        {
            var notificationLink = new NotificationUserLink()
            {
                UserId = userId,
                NotificationId = notificationId,
                ReadAt = null,
                SentAt = DateTimeOffset.UtcNow,
                TemplateValues = templateValues
            };
            await _dbCtx.NotificationUserLinks.AddAsync(notificationLink);
            await _dbCtx.SaveChangesAsync();
            return Result.Ok(MapToNotification(notificationData, templateValues, notificationLink.SentAt));
        });
    }

    public async Task<Result<Notification>> CreateAndAssignToUser(Guid userId, NotificationData notification, Dictionary<string, string> templateValues)
    {
        notification.Visibility = NotificationVisibility.Private;
        var notificationLink = new NotificationUserLink()
        {
            UserId = userId,
            ReadAt = null,
            SentAt = DateTimeOffset.UtcNow,
            TemplateValues = templateValues
        };
        await _dbCtx.NotificationsData.AddAsync(notification);
        notification.NotificationUserLinks.Add(notificationLink);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(MapToNotification(notification, templateValues, notificationLink.SentAt));
    }

    public async Task<Result<Tuple<int, Notification>>> CreateAndAssignToAllUsers(NotificationData notificationData, Dictionary<string, string> templateValues)
    {
        notificationData.Visibility = NotificationVisibility.Public;
        await _dbCtx.NotificationsData.AddAsync(notificationData);
        await _dbCtx.Users.Where(u => !u.IsAnonymous).ForEachAsync((u) =>
        {
            notificationData.NotificationUserLinks.Add(new()
            {
                UserId = u.Id,
                ReadAt = null,
                SentAt = DateTimeOffset.UtcNow,
                TemplateValues = templateValues
            });
        });
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(new Tuple<int, Notification>(notificationData.NotificationUserLinks.Count, MapToNotification(notificationData, templateValues)));
    }

    public async Task<Result<Notification>> CreateAndAssignToAnonymousUsers(NotificationData notification)
    {
        notification.Visibility = NotificationVisibility.Anonymous;
        await _dbCtx.NotificationsData.AddAsync(notification);
        await _dbCtx.SaveChangesAsync();
        return Result.Ok(MapToNotification(notification));
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllByUserId(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null)
    {
        List<Notification> notifications = await _dbCtx.NotificationUserLinks
            .Where(ul =>
                    ul.UserId == userId
                        && (isRead == null || (isRead == true && ul.ReadAt != null) || (isRead == false && ul.ReadAt == null))
                    )
            .Include(ul => ul.Notification)
            .Select(ul => new Notification()
            {
                Id = ul.Id,
                Title = ul.Notification.Title,
                Description = ul.Notification.Description,
                ActionPath = ul.Notification.ActionPath,
                ActionType = ul.Notification.ActionType,
                Payload = ul.Notification.Payload,
                SentAt = ul.SentAt,
                ReadAt = ul.ReadAt,
                TemplateValues = new List<Dictionary<string, string>>() { ul.Notification.TemplateValues, ul.TemplateValues },
            })
            .OrderByDescending(ul => ul.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Result.Ok((IEnumerable<Notification>)notifications);
    }

    public async Task<Result<IEnumerable<Notification>>> GetAllAnonymous(int pageSize = 10, int pageNumber = 1)
    {
        NotificationVisibility[] visibilities = [NotificationVisibility.Public, NotificationVisibility.Anonymous];
        List<Notification> notifications = await _dbCtx.NotificationsData.Where(n => visibilities.Contains(n.Visibility))
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(n => MapToNotification(n))
            .ToListAsync();
        return Result.Ok((IEnumerable<Notification>)notifications);
    }

    public async Task<Result<int>> DeleteAllByUserIdAsync(Guid userId)
    {
        var affected = await _dbCtx.NotificationUserLinks.Where(ul => ul.UserId == userId).ExecuteDeleteAsync();
        return affected > 0 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> DeleteByIdsAsync(Guid userId, int notificationId)
    {
        var affected = await _dbCtx.NotificationUserLinks
                .Where(ul => ul.UserId == userId && ul.Id == notificationId)
                .ExecuteDeleteAsync();
        return affected == 1 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> MarkAllAsReadByUserIdAsync(Guid userId)
    {
        var affected = await _dbCtx.NotificationUserLinks.Where(ul => ul.UserId == userId && ul.ReadAt == null).ExecuteUpdateAsync(
           setters => setters
               .SetProperty(ul => ul.ReadAt, DateTimeOffset.UtcNow)
       );
        return affected > 0 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result<int>> MarkAsReadByIdsAsync(Guid userId, int notificationId)
    {
        var affected = await _dbCtx.NotificationUserLinks
            .Where(ul => ul.UserId == userId && ul.Id == notificationId)
            .ExecuteUpdateAsync(
            setters => setters
                .SetProperty(ul => ul.ReadAt, DateTimeOffset.UtcNow)
            );
        return affected == 1 ?
            Result.Ok(affected) :
            Result.Fail<int>(NotFoundError);
    }

    public async Task<Result> ApplyAnonymousClickById(int notificationId)
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