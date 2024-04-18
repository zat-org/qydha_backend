namespace Qydha.Domain.Services.Implementation;

public class BalootEventsTypesProvider
{
    public Dictionary<string, Type> BalootGameEventsTypes { get; set; } = [];
    public BalootEventsTypesProvider()
    {
        var assembly = typeof(BalootGameEvent).Assembly;
        var eventClasses = assembly.GetExportedTypes()
            .Where(t => t.BaseType is not null && t.BaseType.Equals(typeof(BalootGameEvent)))
            .Where(t => !t.IsAbstract)
            .ToList();
        eventClasses.ForEach(t => BalootGameEventsTypes.Add(t.Name, t));
    }

}
