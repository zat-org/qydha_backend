namespace Qydha.Domain.Entities;

public class NotificationUserLink
{
    public long Id { get; set; }

    public int NotificationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime SentAt { get; set; }

    public virtual NotificationData Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
