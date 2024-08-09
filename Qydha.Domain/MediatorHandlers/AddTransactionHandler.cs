using Microsoft.AspNetCore.SignalR;
using Qydha.Domain.Mappers;

namespace Qydha.Domain.MediatorHandlers;

public class AddPurchaseHandler(INotificationService notificationService) : INotificationHandler<AddTransactionNotification>
{
    private readonly INotificationService _notificationService = notificationService;
    public async Task Handle(AddTransactionNotification notification, CancellationToken cancellationToken)
    {
        int notificationId = SystemDefaultNotifications.MakePurchase;
        switch (notification.Type)
        {
            case TransactionType.Purchase:
                notificationId = SystemDefaultNotifications.MakePurchase;
                break;
            case TransactionType.PromoCode:
                notificationId = SystemDefaultNotifications.UseTicket;
                break;
            case TransactionType.InfluencerCode:
                notificationId = SystemDefaultNotifications.UseInfluencerCode;
                break;
            case TransactionType.Refund:
                notificationId = SystemDefaultNotifications.RefundPurchase;
                break;
        }
        await _notificationService.SendToUserPreDefinedNotification(notification.User.Id, notificationId, []);
    }
}
