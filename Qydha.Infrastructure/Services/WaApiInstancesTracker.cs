using Open.Collections.Synchronized;


namespace Qydha.Infrastructure.Services;

public class WaApiInstancesTracker
{
    private readonly ILogger<WaApiInstancesTracker> _logger;
    private readonly WaApiSettings _waApiSettings;
    private readonly ConcurrentList<WaApiInstance> Instances = [];

    public WaApiInstancesTracker(IOptions<WaApiSettings> settings, ILogger<WaApiInstancesTracker> logger)
    {
        _logger = logger;
        _waApiSettings = settings.Value;
        settings.Value.InstancesIds.ForEach(i => Instances.Add(new WaApiInstance(i)));
    }

    public WaApiInstance? DequeueInstance()
    {
        WaApiInstance? currentInstance = Instances.FirstOrDefault();
        if (currentInstance != null)
        {
            Instances.Remove(currentInstance);
            if (currentInstance.LastUsedAt == null)
                return currentInstance;
            else
            {
                var numOfSeconds = (DateTime.UtcNow - currentInstance.LastUsedAt).Value.Seconds;
                if (numOfSeconds >= _waApiSettings.InstanceMessageInterval)
                    return currentInstance;
                else
                    Instances.Insert(0, currentInstance);
            }
        }
        return null;
    }

    public void EnqueueInstance(WaApiInstance instance)
    {
        instance.LastUsedAt = DateTime.UtcNow;
        Instances.Add(instance);
    }
}
public class WaApiInstance(int instanceId)
{
    public readonly int InstanceId = instanceId;
    public DateTime? LastUsedAt { get; set; }
}