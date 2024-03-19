using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class UserHandSetting
{
    public Guid UserId { get; set; }

    public int? RoundsCount { get; set; }

    public int? MaxLimit { get; set; }

    public int? TeamsCount { get; set; }

    public int? PlayersCountInTeam { get; set; }

    public bool? WinUsingZat { get; set; }

    public int TakweeshPoints { get; set; }

    public virtual User User { get; set; } = null!;
}
