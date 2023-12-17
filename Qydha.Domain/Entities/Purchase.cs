namespace Qydha.Domain.Entities;

[Table("purchases")]
public class Purchase
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("iaphub_purchase_id")]
    public string IAPHubPurchaseId { get; set; } = string.Empty;
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("type")]
    public string Type { get; set; } = string.Empty;
    [Column("purchase_date")]
    public DateTime PurchaseDate { get; set; }
    [Column("productsku")]
    public string ProductSku { get; set; } = string.Empty;
    [Column("number_of_days")]
    public int NumberOfDays { get; set; }
}
