using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class Admin
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string NormalizedUsername { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Role { get; set; } = null!;
}
