namespace Qydha.Domain.Entities;
[Table("InfluencerCodes_categories")]
[NotFoundError(ErrorType.InfluencerCodeCategoryNotFound)]

public class InfluencerCodeCategory : DbEntity<InfluencerCodeCategory>
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    private string categoryName = null!;
    [Column("category_name")]
    public string CategoryName
    {
        get
        {
            return categoryName;
        }
        set
        {
            categoryName = value.Trim().ToUpper();
        }
    }
    [Column("max_codes_per_user_in_group")]
    public int MaxCodesPerUserInGroup { get; set; }

}
