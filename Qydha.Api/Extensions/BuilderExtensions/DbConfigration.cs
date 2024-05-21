using Microsoft.EntityFrameworkCore;

namespace Qydha.API.Extensions;

public static class DbConfigurationExtension
{
    public static void DbConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("postgres");
        services.AddDbContext<QydhaContext>(
            (opt) =>
            {
                opt.UseNpgsql(connectionString, b => b.MigrationsAssembly("Qydha.Api"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging();
            }
        );
    }
}
