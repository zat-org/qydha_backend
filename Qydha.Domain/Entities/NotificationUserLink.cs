namespace Qydha.Domain.Entities;

public class NotificationUserLink
{
    public long Id { get; set; }

    public int NotificationId { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    public DateTimeOffset SentAt { get; set; }

    public Dictionary<string, string> TemplateValues = [];

    public virtual NotificationData Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;

}
