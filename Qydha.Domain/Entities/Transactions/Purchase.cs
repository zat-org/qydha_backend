namespace Qydha.Domain.Entities;
public class Purchase
{
    public Guid Id { get; set; }
    public string IAPHubPurchaseId { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Type { get; set; } = null!;
    public DateTimeOffset PurchaseDate { get; set; }
    public string ProductSku { get; set; } = null!;
    public int NumberOfDays { get; set; }
    public DateTimeOffset? RefundedAt { get; set; }
    public virtual User User { get; set; } = null!;
}
