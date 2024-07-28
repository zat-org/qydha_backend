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
                .WithOrigins("http://localhost:5173")
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
