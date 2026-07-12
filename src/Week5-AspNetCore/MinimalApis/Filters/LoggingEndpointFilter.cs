using System.Diagnostics;

namespace MinimalApis.Filters;

/// <summary>
/// Reusable <see cref="IEndpointFilter"/> that logs the route, elapsed time, and
/// resulting status for every request that passes through it. Attached once per
/// <see cref="RouteGroupBuilder"/> instead of duplicating logging code per handler.
/// </summary>
public sealed class LoggingEndpointFilter : IEndpointFilter
{
    private readonly ILogger<LoggingEndpointFilter> _logger;

    public LoggingEndpointFilter(ILogger<LoggingEndpointFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await next(context);
        stopwatch.Stop();

        _logger.LogInformation(
            "{Method} {Path} -> {ResultType} in {ElapsedMilliseconds} ms",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            result?.GetType().Name ?? "null",
            stopwatch.ElapsedMilliseconds);

        return result;
    }
}
