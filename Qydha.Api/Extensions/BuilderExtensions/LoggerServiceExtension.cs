using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;

namespace Qydha.API.Extensions;

public static class LoggerServiceExtension
{
    public static void AddLoggerConfiguration(ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(configuration)
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", environment.IsProduction() ? LogEventLevel.Warning : LogEventLevel.Information)
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(renderMessage: true), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning, retainedFileTimeLimit: TimeSpan.FromDays(30));

        if (environment.IsDevelopment())
        {
            loggerConfig.WriteTo.File(new JsonFormatter(renderMessage: true), "./Info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information, retainedFileTimeLimit: TimeSpan.FromDays(7));
        }
        else
        {
            GoogleLoggerSettings googleLoggerConfig = configuration.GetSection("GoogleLogger").Get<GoogleLoggerSettings>()
                ?? throw new ArgumentNullException("can't get logging Configuration.");
            string serviceAccountCredential = File.ReadAllText(googleLoggerConfig.JsonKeyPath);
            var googleCloudConfig = new GoogleCloudLoggingSinkOptions(
                projectId: googleLoggerConfig.ProjectId,
                googleCredentialJson: serviceAccountCredential)
            { };
            loggerConfig.WriteTo.GoogleCloudLogging(googleCloudConfig);
        }
        Log.Logger = loggerConfig.CreateLogger();
    }

}
