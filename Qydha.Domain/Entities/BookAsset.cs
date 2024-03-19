namespace Qydha.Domain.Entities;

public class BookAsset
{
    public DateTime LastUpdateAt { get; set; }
    public string Path { get; set; } = null!;
    public string Url { get; set; } = null!;
}
