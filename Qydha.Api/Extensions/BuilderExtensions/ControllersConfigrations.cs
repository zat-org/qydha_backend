using Newtonsoft.Json.Converters;

namespace Qydha.API.Extensions;

public static class ControllersConfigrations
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {


        services.AddControllers((options) =>
        {
            options.Filters.Add<ExceptionHandlerAttribute>();
            options.Filters.Add<AuthorizationFilter>();
            options.ModelBinderProviders.Insert(0, new BalootGameEventsDtoModelBinderProvider());
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        return services;
    }
}
