
namespace Qydha.Domain.MediatorHandlers;

public class UserRegistrationNotificationHandler(INotificationService notificationService) : INotificationHandler<UserRegistrationNotification>
{
    private readonly INotificationService _notificationService = notificationService;
    public async Task Handle(UserRegistrationNotification notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendToUserPreDefinedNotification(notification.User, SystemDefaultNotifications.Register, []);
    }
}
public class UserRegistrationPromoCodeGiftHandler(IUserPromoCodesService userPromoCodesService, IOptions<RegisterGiftSetting> newUserGiftOptions) : INotificationHandler<UserRegistrationNotification>
{
    private readonly IUserPromoCodesService _userPromoCodesService = userPromoCodesService;
    private readonly RegisterGiftSetting _newUserGiftSettings = newUserGiftOptions.Value;
    public async Task Handle(UserRegistrationNotification notification, CancellationToken cancellationToken)
    {
        await _userPromoCodesService.AddPromoCode(notification.User.Id, _newUserGiftSettings.CodeName, _newUserGiftSettings.NumberOfGiftedDays, DateTimeOffset.UtcNow.AddDays(_newUserGiftSettings.ExpireAfterInDays));
    }
}