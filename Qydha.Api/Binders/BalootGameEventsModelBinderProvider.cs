using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Qydha.API.Binders;
public class BalootGameEventsDtoModelBinderProvider() : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(List<BalootGameEventDto>))
        {
            var converter = new BalootGameEventDtoConverter();
            return new BalootGameEventDtoModelBinder(converter);
        }
        return null;
    }
}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class MapToEventNameAttribute(string eventName) : Attribute
{
    public string EventName { get; } = eventName;
}

public class BalootGameEventDtoConverter : JsonConverter
{
    private readonly Dictionary<string, Type> _typesDictionary = [];
    public BalootGameEventDtoConverter()
    {
        typeof(BalootGameEventDto).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BalootGameEventDto)) && !t.IsAbstract).ToList()
        .ForEach(type =>
        {
            MapToEventNameAttribute mapToEventName = (MapToEventNameAttribute)(Attribute.GetCustomAttribute(type, typeof(MapToEventNameAttribute))
                ?? throw new ArgumentException($"{type.Name} has no mapping event name."));

            _typesDictionary.Add(mapToEventName.EventName, type);
        });
    }
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string eventName = (string?)jsonObject.GetValue("eventName")
            ?? throw new InvalidBalootGameEventException("'eventName' Property not found");
        if (string.IsNullOrEmpty(eventName))
            throw new InvalidBalootGameEventException("Invalid eventName Property Value");
        if (!_typesDictionary.TryGetValue(eventName, out var type))
            throw new InvalidBalootGameEventException("Invalid eventName Property Value : event Not Found");
        return JsonConvert.DeserializeObject(jsonObject.ToString(), type, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Converters = [new StringEnumConverter()]
        }) ?? throw new InvalidBalootGameEventException("Error In Serializing the event data");
    }

    public override bool CanConvert(Type objectType)
    {
        var res = objectType == typeof(BalootGameEventDto);
        return res;
    }
}
