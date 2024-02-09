namespace Qydha.Domain.MediatorNotifications;

public class AddPurchaseNotification(Guid userId, int notificationId) : INotification
{
    public Guid UserId { get; } = userId;
    public int NotificationId { get; } = notificationId;
}
