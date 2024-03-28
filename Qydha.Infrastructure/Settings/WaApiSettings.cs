namespace Qydha.Infrastructure.Settings;

public class WaApiSettings
{
    public string ApiUrl { get; set; } = null!;
    public string Token { get; set; } = null!;
    public List<int> InstancesIds { get; set; } = [];
    public int InstanceMessageInterval { get; set; }

}
