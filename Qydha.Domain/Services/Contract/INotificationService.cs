namespace Qydha.Domain.Services.Contracts;

public interface INotificationService
{
    Task<Result<User>> SendToUser(Guid userId, NotificationData notification, Dictionary<string, string> templateValues);
    Task<Result<User>> SendToUser(User user, NotificationData notification, Dictionary<string, string> templateValues);
    Task<Result<User>> SendToUserPreDefinedNotification(Guid userId, int notificationId, Dictionary<string, string> templateValues);
    Task<Result<User>> SendToUserPreDefinedNotification(User user, int notificationId, Dictionary<string, string> templateValues);
    Task<Result<int>> SendToAllUsers(NotificationData notification, Dictionary<string, string> templateValues);
    Task<Result<Notification>> SendToAnonymousUsers(NotificationData notification);
    Task<Result> MarkAllNotificationsOfUserAsRead(Guid userId);
    Task<Result> MarkNotificationAsRead(Guid userId, int notificationId);
    Task<Result> DeleteNotification(Guid userId, int notificationId);
    Task<Result> DeleteAll(Guid userId);
    Task<Result<PagedList<Notification>>> GetByUserId(Guid userId, PaginationParameters pageParams);
    Task<Result<PagedList<Notification>>> GetAllAnonymous(PaginationParameters pageParams);
    Task<Result<FileData>> UploadNotificationImage(IFormFile file);
    Task<Result> ApplyAnonymousClick(int notificationId);
}