namespace Qydha.API.Models;

public class GetNotificationDto
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; }
    public string ActionPath { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public Dictionary<string, object> Payload { get; set; } = null!;
}
