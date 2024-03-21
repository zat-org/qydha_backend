using Newtonsoft.Json;

namespace Qydha.Domain.Entities;
public class NotificationData
{
    // ! convert id to long 
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string ActionPath { get; set; } = string.Empty;

    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;

    public Dictionary<string, object> Payload = [];

    public NotificationVisibility Visibility { get; set; } = NotificationVisibility.Private;

    public int AnonymousClicks { get; set; }

    public virtual ICollection<NotificationUserLink> NotificationUserLinks { get; set; } = [];

}
