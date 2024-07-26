namespace Qydha.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class MapToEventNameAttribute(string eventName) : Attribute
{
    public string EventName { get; } = eventName;
}