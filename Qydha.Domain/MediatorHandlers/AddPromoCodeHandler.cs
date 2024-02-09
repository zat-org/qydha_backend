namespace Qydha.Domain.MediatorHandlers;

public class AddPromoCodeHandler(INotificationService notificationService) : INotificationHandler<AddPromoCodeNotification>
{
    private readonly INotificationService _notificationService = notificationService;
    public async Task Handle(AddPromoCodeNotification notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendToUserPreDefinedNotification(notification.UserId, SystemDefaultNotifications.GetTicket);
    }
}
