namespace Qydha.Domain.Services.Contracts;

public interface INotificationService
{
    Task<Result<User>> SendToUser(Notification notification);
    Task<Result<User>> SendToUser(User user, Notification notification);
    Task<Result<int>> SendToAllUsers(Notification notification);
    Task<Result<int>> SendToGroupOfUsers(Notification notification, Func<User, bool> criteriaFunc);
    Task<Result> MarkNotificationAsRead(Guid userId, int notificationId);
    Task<Result> DeleteNotification(Guid userId, int notificationId);
    Task<Result> DeleteAll(Guid userId);
    Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result<FileData>> UploadNotificationImage(IFormFile file);
}