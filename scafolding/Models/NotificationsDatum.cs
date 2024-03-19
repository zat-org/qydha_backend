using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class NotificationsDatum
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? ActionPath { get; set; }

    public short ActionType { get; set; }

    public string Payload { get; set; } = null!;

    public short Visibility { get; set; }

    public int AnonymousClicks { get; set; }

    public virtual ICollection<NotificationsUsersLink> NotificationsUsersLinks { get; set; } = new List<NotificationsUsersLink>();
}
