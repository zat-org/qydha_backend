
namespace Qydha.API.Mappers;
[Mapper]

public partial class NotificationMapper
{

    public NotificationPage PageListToNotificationPageDto(PagedList<Notification> notifications)
    {
        return new NotificationPage(notifications.Select(NotificationToGetNotificationDto).ToList(),
            notifications.TotalCount,
            notifications.CurrentPage,
            notifications.PageSize);
    }

    [MapProperty(nameof(Notification.Id), nameof(GetNotificationDto.NotificationId))]
    [MapperIgnoreSource(nameof(Notification.TemplateValues))]
    public partial GetNotificationDto NotificationToGetNotificationDto(Notification notification);


    [MapProperty(nameof(NotificationData.Id), nameof(GetNotificationDto.NotificationId))]
    [MapProperty(nameof(NotificationData.CreatedAt), nameof(GetNotificationDto.SentAt))]
    [MapperIgnoreSource(nameof(NotificationData.ActionType))]
    [MapperIgnoreSource(nameof(NotificationData.Visibility))]
    [MapperIgnoreSource(nameof(NotificationData.SendingMechanism))]
    [MapperIgnoreSource(nameof(NotificationData.AnonymousClicks))]
    [MapperIgnoreSource(nameof(NotificationData.TemplateValues))]
    [MapperIgnoreSource(nameof(NotificationData.NotificationUserLinks))]
    [MapperIgnoreTarget(nameof(GetNotificationDto.ReadAt))]
    [MapperIgnoreTarget(nameof(GetNotificationDto.ActionType))]
    public partial GetNotificationDto NotificationDataToGetNotificationDto(NotificationData notificationData);
}
