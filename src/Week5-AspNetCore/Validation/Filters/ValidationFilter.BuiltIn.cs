// =============================================================================
// .NET 10 BUILT-IN VALIDATION — No custom IEndpointFilter needed!
// =============================================================================
//
// FluentValidation approach (see ValidationFilter.FV.cs):
//   A custom IEndpointFilter resolves IValidator<T> from DI and manually runs
//   validation. It must be attached to every endpoint that needs it:
//
//     app.MapPost("/products", handler)
//        .AddEndpointFilter<ValidationFilter<CreateProductRequest>>();
//
// .NET 10 built-in approach:
//   Calling builder.Services.AddValidation() registers a framework-provided
//   IEndpointFilter. It is automatically applied to every Minimal API endpoint
//   whose parameters carry DataAnnotations attributes or implement IValidatableObject.
//   You never attach a filter to individual endpoints.
//
//     builder.Services.AddValidation(); // ← that's all
//
//     app.MapPost("/products", (CreateProductRequestAnnotated request) => ...);
//     // No .AddEndpointFilter<>() call here — the framework handles it.
//
// Error response shape:
//   Both approaches return the same RFC 7807 / RFC 9457 ValidationProblem body:
//   {
//     "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
//     "title": "One or more validation errors occurred.",
//     "status": 400,
//     "errors": {
//       "Name":  ["Name is required."],
//       "Price": ["Price must be greater than 0."]
//     }
//   }
//
// This file exists solely to make the side-by-side comparison visible in the
// file tree. It contributes no compilable code to the project.
// =============================================================================
