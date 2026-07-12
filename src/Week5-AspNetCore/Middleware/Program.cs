using Microsoft.AspNetCore.Diagnostics;
using Middleware.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Named, restrictive CORS policy — specific origin/methods/headers only.
// Never combine AllowAnyOrigin() with AllowCredentials(): browsers reject that
// combination, and it would defeat the purpose of an origin allow-list anyway.
const string TrustedClientPolicy = "TrustedClientPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(TrustedClientPolicy, policy =>
    {
        policy.WithOrigins("https://trusted-client.example.com")
              .WithMethods("GET", "POST")
              .WithHeaders("Content-Type", "Authorization");
    });
});

var app = builder.Build();

// ─── Middleware pipeline — registration order matters ───────────────────────
// Exception handler → HTTPS → short-circuit checks → custom logging → routing/endpoints.

// 1) Exception handling — registered first (outermost) so it can catch errors
//    thrown anywhere further down the pipeline, including inside endpoint handlers.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "An unexpected error occurred.",
            status = context.Response.StatusCode,
            detail = error?.Message
        });
    });
});

// 2) HTTPS redirection.
app.UseHttpsRedirection();

// 2b) CORS — after routing-adjacent concerns, before auth/endpoints, so the
//     policy is evaluated for every request but still runs ahead of anything
//     that decides whether the request is authorized.
app.UseCors(TrustedClientPolicy);

// 3) Short-circuiting example — must run before routing so it can abort the
//    request early, before any endpoint code executes.
app.UseMaintenanceMode();

// 4) Custom class-based middleware — logs total request duration. Registered
//    after the checks above so it measures only work that actually happens.
app.UseRequestDuration();

// ─── `Use` vs `Run` vs `Map` ──────────────────────────────────────────────────
// `Use`: inline middleware that calls `next()` to continue the pipeline.
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Powered-By"] = "Week5-Middleware-Demo";
    await next();
});

// `Map`: branches the pipeline for a path prefix — requests to "/branch" never
// reach the endpoints registered below.
app.Map("/branch", branch =>
{
    // `Run`: terminal middleware — never calls `next`, always ends the pipeline.
    branch.Run(async context =>
    {
        await context.Response.WriteAsync("Handled entirely inside the /branch pipeline branch (Map + Run).");
    });
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
}).WithName("GetWeatherForecast");

// Throws so the exception-handling middleware above can be exercised.
app.MapGet("/throw", (string? message) =>
{
    throw new InvalidOperationException(message ?? "Demo exception for the exception-handling middleware.");
}).WithName("ThrowDemoException");

Console.WriteLine("""

  ╔══════════════════════════════════════════════════════════════════╗
  ║  Week 5 — Middleware pipeline demo                                ║
  ╠══════════════════════════════════════════════════════════════════╣
  ║  GET  /weatherforecast     → normal request (timed + logged)     ║
  ║  GET  /throw               → exception handler middleware        ║
  ║  GET  /branch               → Map + Run terminal branch          ║
  ║  Header "X-Maintenance: true" on any request → 503 short-circuit ║
  ║  CORS: only https://trusted-client.example.com (GET/POST) allowed ║
  ╚══════════════════════════════════════════════════════════════════╝
""");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
