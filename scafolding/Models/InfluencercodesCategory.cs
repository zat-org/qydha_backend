using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class InfluencercodesCategory
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public int MaxCodesPerUserInGroup { get; set; }

    public virtual ICollection<Influencercode> Influencercodes { get; set; } = new List<Influencercode>();
}
