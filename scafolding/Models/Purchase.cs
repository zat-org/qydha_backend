using System;
using System.Collections.Generic;

namespace scafolding.Models;

public partial class Purchase
{
    public Guid Id { get; set; }

    public string? IaphubPurchaseId { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime PurchaseDate { get; set; }

    public string Productsku { get; set; } = null!;

    public int NumberOfDays { get; set; }

    public virtual User User { get; set; } = null!;
}
