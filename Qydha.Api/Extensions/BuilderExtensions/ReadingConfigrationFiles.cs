namespace Qydha.API.Extensions;

public static class ReadingConfigrationFiles
{
    public static void ReadConfigrationFile(this WebApplicationBuilder builder)
    {

        builder.Configuration.AddJsonFile("app_keys.json");
        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddJsonFile("app_keys.Development.json");
        else if (builder.Environment.IsStaging())
            builder.Configuration.AddJsonFile("app_keys.Staging.json");
    }
}
// config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: 
// true)
//  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: 
// true, reloadOnChange: true);