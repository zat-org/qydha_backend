using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;


namespace Qydha.API.Binders;

public class BalootGameEventDtoModelBinder(JsonConverter converter) : IModelBinder
{
    private readonly JsonConverter _converter = converter;
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        using var sr = new StreamReader(bindingContext.HttpContext.Request.Body);
        string json = await sr.ReadToEndAsync();
        try
        {
            var val = JsonConvert.DeserializeObject<List<BalootGameEventDto>>(json, _converter);
            bindingContext.Result = ModelBindingResult.Success(val);
        }
        catch (JsonSerializationException exp)
        {
            throw new InvalidBalootGameEventException(exp.Message, exp);
        }
    }
}
