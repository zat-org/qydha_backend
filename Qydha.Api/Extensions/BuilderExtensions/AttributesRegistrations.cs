
namespace Qydha.API.Extensions;

public static class AttributesRegistrations
{
    public static IServiceCollection RegisterAttributes(this IServiceCollection services)
    {
        services.AddScoped<ExceptionHandlerAttribute>();
        return services;
    }
}
