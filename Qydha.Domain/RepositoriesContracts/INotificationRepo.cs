namespace Qydha.Domain.Repositories;

public interface INotificationRepo
{
    Task<Result<Notification>> AddAsync(Notification notification);
    Task<Result> DeleteByIdAsync(Guid userId, int id);
    Task<Result<int>> DeleteAllByUserIdAsync(Guid userId);
    Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, Func<Notification, bool> filterCriteria, int pageSize = 10, int pageNumber = 1);
    Task<Result> MarkNotificationAsRead(Guid userId, int id);
    Task<Result<int>> AddToUsersWithCriteria(Notification notification, string filteringCriteria = "");
    Task<Result<int>> AddToUsersWithByIds(Notification notification, IEnumerable<Guid> ids);
    Task<Result> PatchById<T>(Guid userId, int id, string propName, T propValue);
}
