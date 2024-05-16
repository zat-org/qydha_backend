namespace Qydha.API.Extensions;

public static class MediatRConfigrations
{
    public static IServiceCollection ConfigreMediatR(this IServiceCollection services)
    {

        services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(User).Assembly);
            });
        return services;

    }
}
