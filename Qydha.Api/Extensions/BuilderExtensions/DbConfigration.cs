using Microsoft.EntityFrameworkCore;

namespace Qydha.API.Extensions;

public static class DbConfigration
{
    public static void ConfigureDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("postgres");

        // db connection
        builder.Services.AddDbContext<QydhaContext>(
            (opt) =>
            {
                opt.UseNpgsql(connectionString, b => b.MigrationsAssembly("Qydha.Api"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging();
            }
        );
    }

}
