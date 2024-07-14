using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Qydha.API.Binders;

public class BalootGameEventDtoListModelBinder() : IModelBinder
{
    private readonly JsonConverter _converter = new BalootGameEventDtoConverter();
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        using var sr = new StreamReader(bindingContext.HttpContext.Request.Body);
        string json = await sr.ReadToEndAsync();
        try
        {
            // var x = bindingContext.ModelType;
            var val = JsonConvert.DeserializeObject<List<BalootGameEventDto>>(json, _converter);
            bindingContext.Result = ModelBindingResult.Success(val);
        }
        catch (JsonSerializationException exp)
        {
            throw new InvalidBalootGameEventException(exp.Message, exp);
        }
    }
}
public class CreateBalootGameDtoModelBinder() : IModelBinder
{
    private readonly JsonConverter _converter = new BalootGameEventDtoConverter();
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        using var sr = new StreamReader(bindingContext.HttpContext.Request.Body);
        string json = await sr.ReadToEndAsync();
        try
        {
            // var x = bindingContext.ModelType;
            var val = JsonConvert.DeserializeObject<CreateBalootGameDto>(json, _converter);
            bindingContext.Result = ModelBindingResult.Success(val);
        }
        catch (JsonSerializationException exp)
        {
            throw new InvalidBalootGameEventException(exp.Message, exp);
        }
    }
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
        return objectType == typeof(BalootGameEventDto);
    }
}
