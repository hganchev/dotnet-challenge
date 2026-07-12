namespace Middleware.Middlewares;

/// <summary>
/// Demonstrates short-circuiting the pipeline: when the "X-Maintenance: true" header is
/// present, the request never reaches routing or endpoints — a 503 is written directly
/// and <c>_next</c> is never invoked.
/// </summary>
public sealed class MaintenanceModeMiddleware
{
    private const string MaintenanceHeaderName = "X-Maintenance";

    private readonly RequestDelegate _next;

    public MaintenanceModeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(MaintenanceHeaderName, out var value) &&
            value == "true")
        {
            // Short-circuit: write the response and return WITHOUT calling _next().
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { message = "Service is under maintenance. Try again later." });
            return;
        }

        await _next(context);
    }
}
