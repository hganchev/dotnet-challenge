using System.Diagnostics;

namespace Middleware.Middlewares;

/// <summary>
/// Class-based custom middleware that measures and logs how long each request takes.
/// The constructor captures the next <see cref="RequestDelegate"/> in the pipeline once
/// (middleware instances are singletons); <see cref="InvokeAsync"/> runs per request.
/// </summary>
public sealed class RequestDurationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestDurationMiddleware> _logger;

    public RequestDurationMiddleware(RequestDelegate next, ILogger<RequestDurationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation(
            "{Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
