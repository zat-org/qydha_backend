
namespace Qydha.API.Mappers;
[Mapper]

public partial class NotificationMapper
{
    [MapProperty(nameof(Notification.Notification_Id), nameof(GetNotificationDto.NotificationId))]
    [MapProperty(nameof(Notification.Read_At), nameof(GetNotificationDto.ReadAt))]
    [MapProperty(nameof(Notification.Created_At), nameof(GetNotificationDto.CreatedAt))]
    [MapProperty(nameof(Notification.Action_Type), nameof(GetNotificationDto.ActionType))]
    [MapProperty(nameof(Notification.Action_Path), nameof(GetNotificationDto.ActionPath))]
    [MapProperty(nameof(Notification.Title), nameof(GetNotificationDto.Title))]
    [MapProperty(nameof(Notification.Description), nameof(GetNotificationDto.Description))]
    public partial GetNotificationDto NotificationToGetNotificationDto(Notification notification);
}
