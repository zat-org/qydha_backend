using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;

namespace Qydha.API.Extensions;

public static class LoggerServiceExtension
{
    public static void AddLoggerService(this WebApplicationBuilder builder)
    {
        #region Serilog
        var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",
                 builder.Environment.IsProduction() ? LogEventLevel.Warning : LogEventLevel.Information)
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(renderMessage: true), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning);

        if (builder.Environment.IsProduction())
        {
            string serviceAccountCredential = File.ReadAllText("googleCloud_private_key.json");
            var googleLoggerConfig = builder.Configuration.GetSection("GoogleLogger");
            var googleCloudConfig = new GoogleCloudLoggingSinkOptions
            {
                ProjectId = googleLoggerConfig["ProjectId"],
                GoogleCredentialJson = serviceAccountCredential,
                ServiceName = googleLoggerConfig["ServiceName"]
            };
            loggerConfig.WriteTo.GoogleCloudLogging(googleCloudConfig);
        }
        else
        {
            loggerConfig.WriteTo.File(new JsonFormatter(renderMessage: true), "./Info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information);
        }

        Log.Logger = loggerConfig.CreateLogger();
        builder.Host.UseSerilog();
        #endregion
    }

}
