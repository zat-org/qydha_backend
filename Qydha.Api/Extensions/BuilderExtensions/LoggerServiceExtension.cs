using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;

namespace Qydha.API.Extensions;

public static class LoggerServiceExtension
{
    public static void AddLoggerService(this WebApplicationBuilder builder)
    {
        var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", builder.Environment.IsProduction() ? LogEventLevel.Warning : LogEventLevel.Information)
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(renderMessage: true), "./Error_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning, retainedFileTimeLimit: TimeSpan.FromDays(30));

        if (builder.Environment.IsProduction())
        {
            string serviceAccountCredential = File.ReadAllText("googleCloud_private_key.json");
            var googleLoggerConfig = builder.Configuration.GetSection("GoogleLogger");
            var googleCloudConfig = new GoogleCloudLoggingSinkOptions(
                projectId: googleLoggerConfig["ProjectId"],
                googleCredentialJson: serviceAccountCredential,
                serviceName: googleLoggerConfig["ServiceName"],
                resourceLabels: new Dictionary<string, string>() { { "logging_source", googleLoggerConfig["ServiceName"] ?? "qydha" } },
                labels: new Dictionary<string, string>() { { "logging_source", googleLoggerConfig["ServiceName"] ?? "qydha" } }
            )
            { };
            loggerConfig.WriteTo.GoogleCloudLogging(googleCloudConfig);
        }
        else
        {
            loggerConfig.WriteTo.File(new JsonFormatter(renderMessage: true), "./Info_logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information, retainedFileTimeLimit: TimeSpan.FromDays(7));
        }

        Log.Logger = loggerConfig.CreateLogger();
        builder.Host.UseSerilog();
    }

}
