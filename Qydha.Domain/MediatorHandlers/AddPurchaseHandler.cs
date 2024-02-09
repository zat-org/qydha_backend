namespace Qydha.Domain.MediatorHandlers;

public class AddPurchaseHandler(INotificationService notificationService) : INotificationHandler<AddPurchaseNotification>
{
    private readonly INotificationService _notificationService = notificationService;
    public async Task Handle(AddPurchaseNotification notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendToUserPreDefinedNotification(notification.UserId, notification.NotificationId);
    }
}
