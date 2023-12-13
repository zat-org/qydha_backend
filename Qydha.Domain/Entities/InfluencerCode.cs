namespace Qydha.Domain.Entities;
public class InfluencerCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Normalized_Code { get; set; } = null!;
    public DateTime Created_At { get; set; }
    public DateTime? Expire_At { get; set; }
    public int Number_Of_Days { get; set; }

    public InfluencerCode()
    {

    }
    public InfluencerCode(string code, int numOfDays, DateTime? expireDate)
    {
        Code = code;
        Normalized_Code = code.ToUpper();
        Created_At = DateTime.UtcNow;
        Expire_At = expireDate;
        Number_Of_Days = numOfDays;
    }
}