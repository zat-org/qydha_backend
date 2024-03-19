using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class NotificationsUsersLink
{
    public long Id { get; set; }

    public int NotificationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime SentAt { get; set; }

    public virtual NotificationsDatum Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
