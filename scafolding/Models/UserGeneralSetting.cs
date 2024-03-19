using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class UserGeneralSetting
{
    public Guid UserId { get; set; }

    public bool? EnableVibration { get; set; }

    public string? PlayersNames { get; set; }

    public string? TeamsNames { get; set; }

    public virtual User User { get; set; } = null!;
}
