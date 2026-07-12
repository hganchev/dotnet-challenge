# Week 5 - ASP.NET Core

## Goal
Understand the core building blocks of an ASP.NET Core web application: how requests flow through the middleware pipeline, how the built-in DI container manages service lifetimes, how Minimal APIs expose endpoints cleanly, how JWT-based auth is wired up, and how .NET 10's built-in validation compares to FluentValidation.

---

## 1. Middleware

### Topics to Cover
| Topic | Key Points |
|---|---|
| Pipeline model | Middleware chain; each component calls `next()` or short-circuits |
| Registration order | Exception handler → HTTPS → Static files → Routing → Auth → Endpoints |
| `Use` vs `Run` vs `Map` | `Use` passes to next; `Run` is terminal; `Map` branches on path |
| Short-circuiting | Return early without calling `next()` to abort the pipeline |
| Custom middleware class | `InvokeAsync(HttpContext, RequestDelegate)`, injected via constructor |
| Exception handling | `app.UseExceptionHandler` / `app.MapProblemDetails()` as outermost layer |
| Request/response delegates | `RequestDelegate` type, inline lambdas with `app.Use` |

### Expected Outcomes
- Draw the middleware pipeline and explain execution order for a typical request
- Write a custom middleware class that logs request duration
- Explain why exception-handling middleware must be registered first

---

## 2. Dependency Injection

### Topics to Cover
| Topic | Key Points |
|---|---|
| Service lifetimes | **Transient** — new per call; **Scoped** — new per HTTP request; **Singleton** — shared forever |
| Captive dependency | Never inject Scoped/Transient into a Singleton — causes stale state bugs |
| `IServiceCollection` | Register via `AddSingleton`, `AddScoped`, `AddTransient` or extension methods |
| `IServiceProvider` | Resolve at runtime; prefer constructor injection over service-locator |
| `IOptions<T>` | Bind configuration sections to strongly-typed classes; avoid raw `IConfiguration` in business logic |
| `IOptionsSnapshot<T>` | Re-reads config per request (Scoped); useful for reloadable settings |
| Registration extensions | Encapsulate feature registrations in `AddMyFeature(this IServiceCollection s)` |
| Keyed services (.NET 8+) | `AddKeyedSingleton` / `[FromKeyedServices]` for multiple implementations of one interface |

### Expected Outcomes
- Choose the correct lifetime for a given service and justify the choice
- Identify and fix a captive dependency bug
- Implement an `IServiceCollection` extension method to encapsulate feature registration

---

## 3. Minimal APIs

### Topics to Cover
| Topic | Key Points |
|---|---|
| `MapGet/Post/Put/Delete` | Inline lambda handlers; route parameters and query strings |
| `MapGroup` | Group related endpoints under a common prefix; attach shared metadata/filters |
| `TypedResults` | `Results.Ok<T>`, `Results.NotFound()`, `Results.ValidationProblem()` for accurate OpenAPI response types |
| `IResult` return type | Return `IResult` from handlers; use record types for request/response DTOs |
| Endpoint filters | `IEndpointFilter` — runs before/after handlers; used for validation, logging |
| Metadata & docs | `.WithName()`, `.WithTags()`, `.WithOpenApi()` for discoverable API documentation |
| Route groups + auth | `.RequireAuthorization()` on a group applies to all child endpoints |
| Parameter binding | From route, query, body (`[FromBody]`), header, services (`[FromServices]`) |

### Expected Outcomes
- Build a CRUD API using only Minimal API route handlers
- Implement a reusable `IEndpointFilter` for validation or logging
- Group endpoints and apply shared policy without repeating metadata

---

## 4. Authentication & Authorization

### Topics to Cover
| Topic | Key Points |
|---|---|
| Authentication vs authorization | AuthN = who are you; AuthZ = what can you do |
| `AddAuthentication` | Register scheme(s): `AddJwtBearer`, `AddCookie`, etc. |
| JWT validation | `TokenValidationParameters`: issuer, audience, signing key, expiry |
| Claims-based identity | `ClaimsPrincipal`, `ClaimTypes`, reading claims from token |
| Authorization policies | `AddAuthorization(o => o.AddPolicy(...))`, claims/roles requirements |
| `RequireAuthorization` | Apply named policy to endpoint or group; avoid per-handler attribute sprawl |
| Token expiry | Always set `exp`; use short-lived access tokens with refresh tokens |
| Secrets management | Never commit JWT keys to source control — use env vars or a secrets manager |

### Expected Outcomes
- Configure JWT bearer authentication end-to-end (issue + validate a token)
- Define a named policy based on a claim and apply it to a route group
- Explain the difference between `[Authorize]` attribute and `RequireAuthorization()` in Minimal APIs

---

## 5. Validation (FluentValidation vs .NET 10 Built-in)

> Project: `src/Week5-AspNetCore/Validation/` — run with `dotnet run -f net9.0` or `dotnet run -f net10.0`

### Topics to Cover
| Topic | Key Points |
|---|---|
| FluentValidation | `AbstractValidator<T>`, rule chaining, `.When()` / `.Must()` for cross-field rules |
| `IEndpointFilter` (FV) | Generic filter resolves `IValidator<T>` from DI; short-circuits with `ValidationProblem` |
| DI registration (FV) | `AddValidatorsFromAssemblyContaining<Program>()` scans assembly for validators |
| DataAnnotations (built-in) | `[Required]`, `[MaxLength]`, `[Range]`, `[AllowedValues]` on DTO parameters |
| `IValidatableObject` | Cross-field rules without a separate validator class; called after attribute checks pass |
| `AddValidation()` (.NET 10) | Registers framework-level auto-validation filter; no per-endpoint setup needed |
| Error response shape | Both approaches return RFC 9457 `ValidationProblem` — identical response body |

### Key Comparison

| Aspect | FluentValidation (net9.0) | .NET 10 Built-in (net10.0) |
|---|---|---|
| NuGet dependency | `FluentValidation.DependencyInjectionExtensions` | None |
| DTO style | Plain record, no attributes | Record with `[Required]` / `[Range]` / `[AllowedValues]` |
| Cross-field rules | `.When()` / `.Must()` in `AbstractValidator<T>` | `IValidatableObject.Validate()` on the DTO |
| Registration | `AddValidatorsFromAssemblyContaining<Program>()` | `builder.Services.AddValidation()` |
| Endpoint wiring | `.AddEndpointFilter<ValidationFilter<T>>()` per endpoint | Automatic — nothing per endpoint |
| Error format | RFC 9457 `ValidationProblem` (explicit) | RFC 9457 `ValidationProblem` (automatic) |

### Expected Outcomes
- Implement a FluentValidation validator with cross-field rules and wire it via `IEndpointFilter`
- Apply .NET 10 built-in validation with DataAnnotations and `IValidatableObject`
- Explain what `AddValidation()` does under the hood and why no filter attachment is needed
- Describe the trade-offs: FV gives richer code-only rules; built-in requires no package but is attribute-driven

---

## Coverage Audit

### Middleware
| Topic | Covered? | File |
|---|---|---|
| Custom middleware pipeline | ✅ | `Middleware/Middlewares/RequestDurationMiddleware.cs` |
| Short-circuiting | ✅ | `Middleware/Middlewares/MaintenanceModeMiddleware.cs` |
| Exception handler | ✅ | `Middleware/Program.cs` (`UseExceptionHandler`) |
| `Use` vs `Run` vs `Map` | ✅ | `Middleware/Program.cs` (`/branch` route) |

### Dependency Injection
| Topic | Covered? | File |
|---|---|---|
| Service lifetimes demo | ✅ | `DependencyInjection/Services/*Service.cs`, `Program.cs` (`/lifetimes`) |
| Captive dependency bug + fix | ✅ | `DependencyInjection/Services/CaptiveDependencyServices.cs` |
| `IOptions<T>` / `IOptionsSnapshot<T>` | ✅ | `DependencyInjection/Options/MyFeatureOptions.cs`, `Program.cs` (`/options`) |
| Registration extension methods | ✅ | `DependencyInjection/Extensions/ServiceCollectionExtensions.cs` |
| Keyed services | ✅ | `DependencyInjection/Services/Greeters.cs`, `Program.cs` (`/greet/{language}`) |

### Minimal APIs
| Topic | Covered? | File |
|---|---|---|
| Route handlers + `MapGroup` | ✅ | `MinimalApis/Endpoints/ProductEndpoints.cs` |
| `TypedResults` | ✅ | `MinimalApis/Endpoints/ProductEndpoints.cs` |
| `IEndpointFilter` | ✅ | `Validation/Filters/ValidationFilter.FV.cs`, `MinimalApis/Filters/LoggingEndpointFilter.cs` |
| Full CRUD + parameter binding | ✅ | `MinimalApis/Endpoints/ProductEndpoints.cs`, `MinimalApis/Repositories/ProductRepository.cs` |

### Authentication & Authorization
| Topic | Covered? | File |
|---|---|---|
| JWT validation setup | ✅ | `Auth/Program.cs` (`AddJwtBearer`), `Auth/Services/TokenService.cs` |
| Named authorization policies | ✅ | `Auth/Program.cs` (`AdminOnly`, `AdultsOnly`) |
| Claims-based identity | ✅ | `Auth/Program.cs` (`/me`), `Auth/Services/TokenService.cs` |

### Validation
| Topic | Covered? | File |
|---|---|---|
| `AbstractValidator<T>` + cross-field rules | ✅ | `Validation/Validators/CreateProductRequestValidator.FV.cs` |
| Generic `IEndpointFilter` (FV) | ✅ | `Validation/Filters/ValidationFilter.FV.cs` |
| DataAnnotations DTO | ✅ | `Validation/Dtos/CreateProductRequestAnnotated.cs` |
| `IValidatableObject` cross-field | ✅ | `Validation/Dtos/CreateProductRequestAnnotated.cs` |
| `AddValidation()` (.NET 10) | ✅ | `Validation/Program.cs` (`#if NET10_0_OR_GREATER`) |
| Side-by-side `Program.cs` | ✅ | `Validation/Program.cs` |

### Gaps to Address
- None — all five Week 5 sub-projects (Middleware, DependencyInjection, MinimalApis, Auth, Validation) are implemented and smoke-tested.
