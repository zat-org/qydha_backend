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
    public int MaxCodesPerUserInGroup { get; set; }
    public ICollection<InfluencerCode> InfluencerCodes { get; set; } = [];

}
