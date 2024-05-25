namespace Qydha.API.Extensions;
public static class UseRequestContextLoggingExtension
{
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        return app;
    }
}