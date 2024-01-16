using Newtonsoft.Json;

namespace Qydha.Domain.Entities;

public class Notification
{

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionPath { get; set; } = string.Empty;
    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;

    public string PayloadStr { get; set; } = null!;
    public Dictionary<string, object> Payload
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, object>>(PayloadStr) ?? [];
        set => PayloadStr = JsonConvert.SerializeObject(value);
    }
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