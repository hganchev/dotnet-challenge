---
applyTo: "src/Week5-AspNetCore/**"
---
# Week 5 — ASP.NET Core

## Middleware
- Register middleware in the correct order: exception handling → HTTPS → static files → routing → auth → endpoints.
- Use `app.UseExceptionHandler` or `app.MapProblemDetails()` as the outermost error middleware — never swallow exceptions silently.
- Short-circuit the pipeline explicitly with early returns when a request cannot be processed (e.g., invalid input, missing auth).
- Keep each middleware class focused on a single cross-cutting concern; extract logic into services injected via DI.

## Dependency Injection
- Register services with the correct lifetime:
  - **Transient** — stateless, cheap to create, short-lived operations.
  - **Scoped** — once per HTTP request; use for `DbContext`, unit-of-work, per-request caches.
  - **Singleton** — shared across the entire application lifetime; must be thread-safe.
- Never inject a Scoped or Transient service into a Singleton — causes captive dependency bugs.
- Inject `IOptions<T>` / `IOptionsSnapshot<T>` for configuration; avoid `IConfiguration` in business logic classes.
- Use `IServiceCollection` extension methods (e.g., `AddMyFeature(this IServiceCollection s)`) to encapsulate registration.

## Minimal APIs
- Group related endpoints with `app.MapGroup("/prefix")` and attach shared filters/metadata there.
- Use `TypedResults` (e.g., `Results.Ok<T>`, `Results.NotFound()`) for accurate OpenAPI response type inference.
- Validate inputs with `IValidateOptions<T>` or a dedicated validator; do not trust raw user input.
- Validate request DTOs with data annotations or FluentValidation and return `Results.ValidationProblem()` on failure — never let unvalidated input reach business logic.
- Return `IResult` from handler lambdas; use record types for request/response DTOs.
- Annotate endpoints with `.WithName()`, `.WithTags()`, and `.WithOpenApi()` for discoverable API docs.
- Configure CORS with a named, restrictive policy (specific origins/methods/headers) — never combine `AllowAnyOrigin()` with credentials.
- Layer configuration sources deliberately (`appsettings.json` → `appsettings.{Environment}.json` → environment variables → user secrets/Key Vault) and know that later sources override earlier ones.

## Authentication & Authorization
- Validate JWT tokens in middleware (`AddJwtBearer`), not inside individual endpoint handlers.
- Use named authorization policies (`RequireAuthorization("PolicyName")`) over attribute-per-endpoint approaches.
- Never store secrets (JWT signing keys, connection strings) in `appsettings.json` committed to source control — use environment variables or a secrets manager.
- Prefer claims-based authorization (`ClaimsPrincipal`) over role strings for fine-grained access control.
- Always set token expiry (`exp`) and validate it — use short-lived access tokens with refresh tokens.
