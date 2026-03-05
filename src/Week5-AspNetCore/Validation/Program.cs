using FluentValidation;
using Validation.Dtos;
using Validation.Filters;

var builder = WebApplication.CreateBuilder(args);

// ─── Validation registration ─────────────────────────────────────────────────
//
// The #if block below is the key architectural difference between the two approaches.
//
#if NET10_0_OR_GREATER
// .NET 10 BUILT-IN VALIDATION
// One call registers a framework-level endpoint filter that auto-validates
// Minimal API parameters decorated with DataAnnotations / IValidatableObject.
// No per-endpoint .AddEndpointFilter<>() needed.
// See: Dtos/CreateProductRequestAnnotated.cs for how the DTO is annotated.
builder.Services.AddValidation();
#else
// FLUENTVALIDATION (net9.0)
// Scans the assembly for AbstractValidator<T> classes and registers them in DI.
// Each endpoint that needs validation must manually attach ValidationFilter<T>.
// See: Validators/CreateProductRequestValidator.FV.cs for the validator class.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
#endif

var app = builder.Build();
// No UseHttpsRedirection — HTTP only for this local demo (Urls = http://localhost:5000)

// ─── Health / discovery ──────────────────────────────────────────────────────
app.MapGet("/", () => TypedResults.Ok(new
{
    Endpoint = "POST /products",
    ValidExample = new { name = "Laptop Pro", price = 999.99, category = "Electronics" },
    InvalidExample = new { name = "", price = -1, category = "Unknown" },
    HintFile = "Validation.http"
})).WithTags("Info");

// ─── Endpoints ───────────────────────────────────────────────────────────────
//
#if NET10_0_OR_GREATER
// .NET 10 BUILT-IN VALIDATION PATH
// The framework validates CreateProductRequestAnnotated automatically because
// AddValidation() was called above. No .AddEndpointFilter<>() on this endpoint.
app.MapPost("/products", (CreateProductRequestAnnotated request) =>
{
    Console.WriteLine($"[BuiltIn] Creating: {request.Name} | {request.Price:C} | {request.Category}");
    return TypedResults.Ok(new { Message = "Product created (.NET 10 built-in).", request.Name });
})
.WithName("CreateProduct")
.WithTags("Products");
#else
// FLUENTVALIDATION PATH (net9.0)
// ValidationFilter<T> resolves IValidator<CreateProductRequest> from DI,
// runs it, and short-circuits with 400 ValidationProblem on failure.
// See: Filters/ValidationFilter.FV.cs
app.MapPost("/products", (CreateProductRequest request) =>
{
    Console.WriteLine($"[FV] Creating: {request.Name} | {request.Price:C} | {request.Category}");
    return TypedResults.Ok(new { Message = "Product created (FluentValidation).", request.Name });
})
.AddEndpointFilter<ValidationFilter<CreateProductRequest>>()
.WithName("CreateProduct")
.WithTags("Products");
#endif

// ─── Startup banner ──────────────────────────────────────────────────────────
// Use $$"""...""" (two $) so single { } are literal and {{expr}} is interpolation.
Console.WriteLine($$"""

  ╔══════════════════════════════════════════════════════════════════╗
  ║  Validation comparison demo                                      ║
  ║  Framework : {{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,-51}}║
  ╠══════════════════════════════════════════════════════════════════╣
  ║  POST http://localhost:5000/products                             ║
  ║                                                                  ║
  ║  Valid body (200):                                               ║
  ║    { "name":"Laptop","price":999.99,"category":"Electronics" }   ║
  ║                                                                  ║
  ║  Invalid body (400 ValidationProblem):                           ║
  ║    { "name":"","price":-1,"category":"Unknown" }                 ║
  ╚══════════════════════════════════════════════════════════════════╝
""");

app.Run();
