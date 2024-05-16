namespace Qydha.API.Extensions;

public static class FiltersRegistrations
{
    public static IServiceCollection RegisterFilters(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationFilter>();
        return services;
    }
}
