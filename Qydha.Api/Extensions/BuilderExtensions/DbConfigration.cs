﻿using Microsoft.EntityFrameworkCore;

namespace Qydha.API.Extensions;

public static class DbConfigurationExtension
{
    public static void DbConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("postgres");
        services.AddDbContext<QydhaContext>(
            (opt) =>
            {
                var options = opt.UseNpgsql(connectionString,
                    o => o.MigrationsAssembly("Qydha.Api")
                          .UseNetTopologySuite()
                    )
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                if (!environment.IsProduction())
                    options.EnableSensitiveDataLogging();
            }
        );
    }
}
