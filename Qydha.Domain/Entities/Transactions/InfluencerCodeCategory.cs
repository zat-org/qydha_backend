namespace Qydha.Domain.Entities;

public class InfluencerCodeCategory 
{
    public int Id { get; set; }
    private string _categoryName = null!;
    public string CategoryName
    {
        get
        {
            return _categoryName;
        }
        set
        {
            _categoryName = value.Trim().ToUpper();
        }
    }
    public int MaxCodesPerUserInGroup { get; set; } // how many user can use from the same category. 
    public ICollection<InfluencerCode> InfluencerCodes { get; set; } = [];

}
