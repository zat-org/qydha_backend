namespace Qydha.Domain.Entities;

[Table("notification")]
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

    [Column("Payload")]
    [JsonField]
    public Dictionary<string, object> Payload { get; set; } = [];

    [Column("Visibility")]
    public NotificationVisibility Visibility { get; set; } = NotificationVisibility.Private;
}
