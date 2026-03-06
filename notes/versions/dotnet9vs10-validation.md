
# Validation — FluentValidation vs .NET 10 Built-in

> Implemented in `src/Week5-AspNetCore/Validation/` — run with `dotnet run -f net9.0` or `dotnet run -f net10.0`.

| Aspect | FluentValidation (net9.0) | .NET 10 Built-in (net10.0) |
|---|---|---|
| NuGet package | `FluentValidation.DependencyInjectionExtensions` | None — framework only |
| DTO style | Plain record, no attributes | `[Required]` `[MaxLength]` `[Range]` `[AllowedValues]` on record parameters |
| Cross-field rules | `.When()` / `.Must()` in `AbstractValidator<T>` | `IValidatableObject.Validate()` on the DTO |
| DI registration | `AddValidatorsFromAssemblyContaining<Program>()` | `builder.Services.AddValidation()` |
| Endpoint wiring | `.AddEndpointFilter<ValidationFilter<T>>()` per endpoint | Automatic — no per-endpoint setup |
| Error format | RFC 9457 `ValidationProblem` (manual) | RFC 9457 `ValidationProblem` (automatic) |
| Test files | `Validators/CreateProductRequestValidator.FV.cs` | `Dtos/CreateProductRequestAnnotated.cs` |

---