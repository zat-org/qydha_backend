using Newtonsoft.Json.Converters;

namespace Qydha.API.Extensions;

public static class ControllersConfigurations
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers((options) =>
        {
            options.Filters.Add<ExceptionHandlerAttribute>();
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        return services;
    }
}
