namespace Middleware.Middlewares;

/// <summary>
/// Encapsulates registration of the custom middleware classes so <c>Program.cs</c>
/// reads as a clear pipeline declaration.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestDuration(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestDurationMiddleware>();

    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app) =>
        app.UseMiddleware<MaintenanceModeMiddleware>();
}
