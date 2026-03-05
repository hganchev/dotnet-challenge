---
applyTo: "src/Week6-EFCore/**"
---
# Week 6 — Entity Framework Core

## DbContext & Migrations
- Keep one `DbContext` per bounded context / database; do not create a single "god context".
- Use `OnModelCreating` with Fluent API for complex configuration; use data annotations only for simple constraints.
- Always name migrations meaningfully (e.g., `AddOrderStatusColumn`) — avoid auto-generated names in long-lived projects.
- Apply migrations at startup with `dbContext.Database.MigrateAsync()` in integration/dev environments; use scripts in production.
- Configure `DbContext` lifetime as **Scoped** (one per HTTP request) when used in ASP.NET Core.

## Query Optimisation
- Use `AsNoTracking()` for read-only queries — eliminates change-tracker overhead and reduces memory usage.
- Avoid N+1 queries: use `Include()` / `ThenInclude()` or split queries (`AsSplitQuery()`) to load related data.
- Use `Select()` projections to retrieve only required columns; never load full entities when only a few fields are needed.
- Use compiled queries (`EF.CompileAsyncQuery`) for frequently executed, performance-critical queries.
- Prefer `ExecuteUpdateAsync` / `ExecuteDeleteAsync` (EF 7+) for bulk operations instead of loading entities into memory.
- Examine generated SQL with `LogTo(Console.WriteLine)` or a logging provider during development.

## Relationships
- Configure relationships explicitly via Fluent API to avoid EF's convention surprises.
- Use `DeleteBehavior.Restrict` or `DeleteBehavior.NoAction` by default; only opt into cascade delete when clearly appropriate.
- Prefer navigation properties with private setters and factory methods on the aggregate root to enforce domain invariants.
- Use `OwnsOne` / `OwnsMany` for value objects embedded in an entity; do not create a separate `DbSet<T>` for them.
- For many-to-many with payload, create an explicit join entity with its own primary key and configure it explicitly.
