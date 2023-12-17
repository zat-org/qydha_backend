namespace Qydha.Domain.Repositories;

public interface INotificationRepo : IGenericRepository<Notification>
{
    Task<Result> DeleteByIdAndUserIdAsync(Guid userId, int id);
    Task<Result<int>> DeleteAllByUserIdAsync(Guid userId);
    Task<Result<IEnumerable<Notification>>> GetAllNotificationsOfUserById(Guid userId, int pageSize = 10, int pageNumber = 1, bool? isRead = null);
    Task<Result> MarkNotificationAsRead(Guid userId, int id);
    Task<Result<int>> AddToUsersWithCriteria(Notification notification, string filteringCriteria = "", object? filterParams = null);
    Task<Result<int>> AddToUsersWithByIds(Notification notification, IEnumerable<Guid> ids);
}
