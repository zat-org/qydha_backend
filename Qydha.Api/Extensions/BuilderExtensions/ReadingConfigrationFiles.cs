namespace Qydha.API.Extensions;

public static class ReadingConfigurationFile
{
    public static void ReadConfigurationFile(ConfigurationManager configuration, IWebHostEnvironment environment)
    {

        configuration.AddJsonFile("app_keys.json");
        if (environment.IsDevelopment())
            configuration.AddJsonFile("app_keys.Development.json");
        else if (environment.IsStaging())
            configuration.AddJsonFile("app_keys.Staging.json");
    }
}
// TODO
// config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: 
// true)
//  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: 
// true, reloadOnChange: true);