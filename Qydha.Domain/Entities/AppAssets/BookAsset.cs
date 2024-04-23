namespace Qydha.Domain.Entities;

public class BookAsset
{
    public DateTimeOffset? LastUpdateAt { get; set; } = null;
    public string Path { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
