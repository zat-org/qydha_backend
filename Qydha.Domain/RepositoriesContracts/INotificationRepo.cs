namespace Qydha.Domain.Repositories;

public interface INotificationRepo
{
    
    Task<Result<int>> AssignNotificationToUser(Guid userId, int notificationId);
    Task<Result<int>> AssignNotificationToUser(Guid userId, NotificationData notification);
    Task<Result<int>> AssignNotificationToAllUsers(NotificationData notification);
    Task<Result<NotificationData>> AssignNotificationToAllAnonymousUsers(NotificationData notification);
    Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result<IEnumerable<NotificationData>>> GetAllAnonymousUserNotification(int pageSize = 10, int pageNumber = 1);
    Task<Result<int>> DeleteAllByUserIdAsync(Guid userId);
    Task<Result<int>> DeleteNotificationByUserIdAsync(Guid userId, int notificationId);
    Task<Result<int>> MarkAllAsReadByUserIdAsync(Guid userId);
    Task<Result<int>> MarkNotificationAsReadByUserIdAsync(Guid userId, int notificationId);
    Task<Result> ApplyAnonymousClickOnNotification(int notificationId);
}
