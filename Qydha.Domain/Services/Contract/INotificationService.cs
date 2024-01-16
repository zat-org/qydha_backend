namespace Qydha.Domain.Services.Contracts;

public interface INotificationService
{
    Task<Result<User>> SendToUser(Guid userId, NotificationData notification);
    Task<Result<User>> SendToUser(User user, NotificationData notification);
    Task<Result<User>> SendToUserPreDefinedNotification(Guid userId, int notificationId);
    Task<Result<User>> SendToUserPreDefinedNotification(User user, int notificationId);
    Task<Result<int>> SendToAllUsers(NotificationData notification);
    // Task<Result<int>> SendToGroupOfUsers(Notification notification, Func<User, bool> criteriaFunc);
    Task<Result> MarkAllNotificationsOfUserAsRead(Guid userId);
    Task<Result> MarkNotificationAsRead(Guid userId, int notificationId);
    Task<Result> DeleteNotification(Guid userId, int notificationId);
    Task<Result> DeleteAll(Guid userId);
    Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result<IEnumerable<NotificationData>>> GetAllAnonymousUserNotification(int pageSize = 10, int pageNumber = 1);
    Task<Result<FileData>> UploadNotificationImage(IFormFile file);
}