using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class UserPromoCode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public short NumberOfDays { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
