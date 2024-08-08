using Microsoft.AspNetCore.SignalR;
using Qydha.Domain.Mappers;

namespace Qydha.Domain.MediatorHandlers;

public class AddPurchaseHandler(INotificationService notificationService, IHubContext<UsersHub, IUserClient> hubContext) : INotificationHandler<AddTransactionNotification>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<UsersHub, IUserClient> _hubContext = hubContext;
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
        var mapper = new UserStreamMapper();

        string serializedUser = JsonConvert.SerializeObject(mapper.UserToUserDto(notification.User), BalootConstants.balootEventsSerializationSettings);
        await _hubContext.Clients.User(notification.User.Id.ToString()).UserDataChanged(serializedUser);
        await _notificationService.SendToUserPreDefinedNotification(notification.User.Id, notificationId, []);
    }
}
