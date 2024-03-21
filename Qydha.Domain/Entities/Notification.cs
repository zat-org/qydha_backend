using Newtonsoft.Json;

namespace Qydha.Domain.Entities;

public class Notification
{

    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionPath { get; set; } = string.Empty;
    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;

    public Dictionary<string, object> Payload = [];
    public Guid UserId { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

}

public static class SystemDefaultNotifications
{
    public const int Register = 1;
    public const int MakePurchase = 2;
    public const int GetTicket = 3;
    public const int UseTicket = 4;
    public const int UseInfluencerCode = 5;
}