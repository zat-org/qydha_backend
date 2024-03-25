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
    Task<Result<IEnumerable<Notification>>> GetByUserId(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result<IEnumerable<Notification>>> GetAllAnonymous(int pageSize = 10, int pageNumber = 1);
    Task<Result<FileData>> UploadNotificationImage(IFormFile file);
    Task<Result> ApplyAnonymousClick(int notificationId);
}