namespace Qydha.API.Extensions;

public static class CorsConfigrations
{
    public static string ConfigureCORS(this WebApplicationBuilder builder)
    {
        #region Add Cors
        string MyAllowSpecificOrigins = "_MyAllowSpecificOrigins";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });
        return MyAllowSpecificOrigins;
        #endregion
    }
}
