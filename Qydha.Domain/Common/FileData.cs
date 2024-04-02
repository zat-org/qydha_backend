using Newtonsoft.Json;

namespace Qydha.Domain.Common;

public class FileData
{
    [JsonProperty(PropertyName = "Url")]
    public string Url { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}
