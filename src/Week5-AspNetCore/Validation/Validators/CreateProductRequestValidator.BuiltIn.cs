// =============================================================================
// .NET 10 BUILT-IN VALIDATION — No separate validator class needed!
// =============================================================================
//
// FluentValidation approach (see CreateProductRequestValidator.FV.cs):
//   Rules live in a dedicated AbstractValidator<T> class.
//   The DTO itself is kept free of validation concerns.
//
// .NET 10 built-in approach:
//   Rules are expressed as DataAnnotations attributes directly on the DTO record
//   (see Dtos/CreateProductRequestAnnotated.cs), plus IValidatableObject for
//   cross-field logic. There is no separate "validator" class at all.
//
// Registration difference:
//
//   FluentValidation (net9.0 path in Program.cs):
//     builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//     // Scans the assembly and registers all AbstractValidator<T> classes.
//     // Endpoints must manually attach ValidationFilter<T>.
//
//   .NET 10 built-in (net10.0 path in Program.cs):
//     builder.Services.AddValidation();
//     // Registers a framework-level IEndpointFilter that auto-validates any
//     // Minimal API parameter with DataAnnotations or IValidatableObject.
//     // No per-endpoint setup needed.
//
// This file exists solely to make the side-by-side comparison visible in the
// file tree. It contributes no compilable code to the project.
// =============================================================================
