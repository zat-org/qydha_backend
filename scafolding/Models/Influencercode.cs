using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class Influencercode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string NormalizedCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpireAt { get; set; }

    public int NumberOfDays { get; set; }

    public int MaxInfluencedUsersCount { get; set; }

    public int? CategoryId { get; set; }

    public virtual InfluencercodesCategory? Category { get; set; }
}
