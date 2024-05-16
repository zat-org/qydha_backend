namespace Qydha.Domain.Entities;
public class Purchase
{
    public Guid Id { get; set; }
    public string IAPHubPurchaseId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset PurchaseDate { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public int NumberOfDays { get; set; }
    public virtual User User { get; set; } = null!;

    
}
