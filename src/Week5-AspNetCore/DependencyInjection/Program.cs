using DependencyInjection.Extensions;
using DependencyInjection.Options;
using DependencyInjection.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMyFeature(builder.Configuration);

// The captive-dependency demo below (IBuggyCaptiveDependencyService) injects a Scoped
// service directly into a Singleton constructor. ASP.NET Core's Development-time
// container guard (ValidateOnBuild/ValidateScopes) would normally refuse to build the
// app for this — which is the framework protecting you. We disable it here ONLY so the
// bug's runtime symptom (a stale, "captured" scoped instance) can be observed instead of
// failing fast at startup. Production does NOT validate this by default either, which is
// exactly why this bug is dangerous in real deployments — see Services/CaptiveDependencyServices.cs.
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false;
    options.ValidateOnBuild = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// ─── Service lifetimes demo ──────────────────────────────────────────────────
// Resolves each service TWICE within the same request to show which lifetimes
// hand back the same instance and which don't:
//  - Transient → different InstanceId both times.
//  - Scoped    → same InstanceId within this request, but changes across requests.
//  - Singleton → same InstanceId always, for the whole app lifetime.
app.MapGet("/lifetimes", (
    ITransientService transientA, ITransientService transientB,
    IScopedService scopedA, IScopedService scopedB,
    ISingletonService singletonA, ISingletonService singletonB) =>
{
    return TypedResults.Ok(new
    {
        Transient = new { First = transientA.InstanceId, Second = transientB.InstanceId, SameInstance = transientA.InstanceId == transientB.InstanceId },
        Scoped = new { First = scopedA.InstanceId, Second = scopedB.InstanceId, SameInstance = scopedA.InstanceId == scopedB.InstanceId },
        Singleton = new { First = singletonA.InstanceId, Second = singletonB.InstanceId, SameInstance = singletonA.InstanceId == singletonB.InstanceId }
    });
}).WithName("GetLifetimes");

// ─── Captive dependency bug vs fix ───────────────────────────────────────────
app.MapGet("/captive-dependency", (
    IBuggyCaptiveDependencyService buggy,
    IFixedCaptiveDependencyService fixedService,
    IScopedService currentScoped) =>
{
    return TypedResults.Ok(new
    {
        CurrentRequestScopedId = currentScoped.InstanceId,
        Buggy = new
        {
            Description = "Singleton captured IScopedService directly in its constructor — this ID never changes across requests.",
            CapturedScopedInstanceId = buggy.CapturedScopedInstanceId
        },
        Fixed = new
        {
            Description = "Singleton uses IServiceScopeFactory to resolve a fresh scope per call — matches the current request's scoped ID.",
            ResolvedScopedInstanceId = fixedService.ResolveCurrentScopedInstanceId()
        }
    });
}).WithName("GetCaptiveDependencyDemo");

// ─── IOptions<T> / IOptionsSnapshot<T> ───────────────────────────────────────
app.MapGet("/options", (IOptions<MyFeatureOptions> options, IOptionsSnapshot<MyFeatureOptions> snapshot) =>
{
    return TypedResults.Ok(new
    {
        ViaIOptions = options.Value,
        ViaIOptionsSnapshot = snapshot.Value
    });
}).WithName("GetOptions");

// ─── Keyed services (.NET 8+) ────────────────────────────────────────────────
app.MapGet("/greet/{language}", (string language, [FromKeyedServices("en")] IGreeter englishGreeter, [FromKeyedServices("bg")] IGreeter bulgarianGreeter) =>
{
    IGreeter greeter = language.ToLowerInvariant() switch
    {
        "bg" => bulgarianGreeter,
        _ => englishGreeter
    };

    return TypedResults.Ok(new { Language = language, Message = greeter.Greet("Developer") });
}).WithName("GetGreeting");

Console.WriteLine("""

  ╔══════════════════════════════════════════════════════════════════╗
  ║  Week 5 — Dependency Injection demo                               ║
  ╠══════════════════════════════════════════════════════════════════╣
  ║  GET  /lifetimes             → Transient vs Scoped vs Singleton   ║
  ║  GET  /captive-dependency    → Singleton capturing Scoped (bug)   ║
  ║  GET  /options               → IOptions<T> / IOptionsSnapshot<T>  ║
  ║  GET  /greet/{en|bg}          → Keyed services ([FromKeyedServices]) ║
  ╚══════════════════════════════════════════════════════════════════╝
""");

app.Run();

