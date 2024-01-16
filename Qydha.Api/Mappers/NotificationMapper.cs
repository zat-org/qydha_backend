
namespace Qydha.API.Mappers;
[Mapper]

public partial class NotificationMapper
{
    [MapProperty(nameof(Notification.Id), nameof(GetNotificationDto.NotificationId))]

    public partial GetNotificationDto NotificationToGetNotificationDto(Notification notification);

    [MapProperty(nameof(NotificationData.Id), nameof(GetNotificationDto.NotificationId))]
    [MapProperty(nameof(NotificationData.CreatedAt), nameof(GetNotificationDto.SentAt))]

    public partial GetNotificationDto NotificationDataToGetNotificationDto(NotificationData notificationData);
}
