
namespace Qydha.API.Mappers;
[Mapper]

public partial class NotificationMapper
{
    [MapProperty(nameof(Notification.Id), nameof(GetNotificationDto.NotificationId))]
    
    public partial GetNotificationDto NotificationToGetNotificationDto(Notification notification);
}
