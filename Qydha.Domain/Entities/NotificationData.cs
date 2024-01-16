using Newtonsoft.Json;

namespace Qydha.Domain.Entities;

[Table("notifications_data")]
[NotFoundError(ErrorType.NotificationNotFound)]
public class NotificationData : DbEntity<NotificationData>
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("action_path")]
    public string ActionPath { get; set; } = string.Empty;

    [Column("action_type")]
    public NotificationActionType ActionType { get; set; } = NotificationActionType.NoAction;


    [Column("payload")]
    [JsonField]
    public string PayloadStr { get; set; } = null!;
    public Dictionary<string, object> Payload
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, object>>(PayloadStr) ?? [];
        set => PayloadStr = JsonConvert.SerializeObject(value);
    }

    [Column("Visibility")]
    public NotificationVisibility Visibility { get; set; } = NotificationVisibility.Private;
}
