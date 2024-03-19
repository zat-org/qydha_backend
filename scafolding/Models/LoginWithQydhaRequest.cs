using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class LoginWithQydhaRequest
{
    public Guid Id { get; set; }

    public string Otp { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
