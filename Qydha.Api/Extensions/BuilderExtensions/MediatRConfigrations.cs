namespace Qydha.API.Extensions;

public static class MediatRConfigurations
{
    public static IServiceCollection ConfigureMediatR(this IServiceCollection services)
    {

        services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(User).Assembly);
            });
        return services;

    }
}
