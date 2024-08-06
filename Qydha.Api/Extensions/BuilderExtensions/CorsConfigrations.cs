namespace Qydha.API.Extensions;

public static class CorsConfigurations
{
    public static string ConfigureCORS(this IServiceCollection services)
    {
        #region Add Cors
        string MyAllowSpecificOrigins = "_MyAllowSpecificOrigins";

        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
            {
                builder
                .WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:3000", "http://localhost:4200", "https://qayedhaadmin.web.app")
                    // .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        return MyAllowSpecificOrigins;
        #endregion
    }
}
