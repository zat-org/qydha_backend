namespace Qydha.API.Models;

public class GetNotificationDto
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public string ActionPath { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public NotificationDataPayload Payload { get; set; } = null!;
}

public class NotificationPage(List<GetNotificationDto> items, int count, int pageNumber, int pageSize)
    : Page<GetNotificationDto>(items, count, pageNumber, pageSize)
{ }