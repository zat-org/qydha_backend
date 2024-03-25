namespace Qydha.Domain.Repositories;

public interface INotificationRepo
{
    Task<Result<NotificationData>> GetByIdAsync(int notificationDataId);
    Task<Result<Notification>> AssignToUser(Guid userId, int notificationId, Dictionary<string, string> templateValues);
    Task<Result<Notification>> CreateAndAssignToUser(Guid userId, NotificationData notification, Dictionary<string, string> templateValues);
    Task<Result<Tuple<int, Notification>>> CreateAndAssignToAllUsers(NotificationData notification, Dictionary<string, string> templateValues);
    Task<Result<Notification>> CreateAndAssignToAnonymousUsers(NotificationData notification);
    Task<Result<IEnumerable<Notification>>> GetAllByUserId(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result<IEnumerable<Notification>>> GetAllAnonymous(int pageSize = 10, int pageNumber = 1);
    Task<Result<int>> DeleteAllByUserIdAsync(Guid userId);
    Task<Result<int>> DeleteByIdsAsync(Guid userId, int notificationId);
    Task<Result<int>> MarkAllAsReadByUserIdAsync(Guid userId);
    Task<Result<int>> MarkAsReadByIdsAsync(Guid userId, int notificationId);
    Task<Result> ApplyAnonymousClickById(int notificationId);
}
