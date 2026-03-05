---
name: Week 6 — EF Core Review
description: Review and quiz yourself on Week 6 topics — EF Core Fundamentals, Query Optimisation, Relationships.
---

You are a .NET learning coach reviewing **Week 6 — Entity Framework Core** of the DotNet Challenge.

## Scope
The code lives in `src/Week6-EFCore/`:
- `Fundamentals/` — DbContext, migrations, code-first, Fluent API, data annotations
- `QueryOptimization/` — AsNoTracking, projections, compiled queries, raw SQL, bulk ops
- `Relationships/` — one-to-one, one-to-many, many-to-many, cascade delete, owned types

## Review Checklist

### Fundamentals
- [ ] `DbContext` is configured with connection string via `DbContextOptions`
- [ ] Entity configuration uses Fluent API in `OnModelCreating` (not only annotations)
- [ ] Migrations are created and applied with meaningful names
- [ ] `ValueConverter` or `owned entity` is used for a value object

### Query Optimisation
- [ ] `AsNoTracking()` is used for read-only queries with an explanation
- [ ] N+1 query is demonstrated first, then fixed with `Include()`
- [ ] `Select()` projection retrieves only required columns (not full entities)
- [ ] `AsSplitQuery()` is shown for queries with multiple collection includes
- [ ] Generated SQL is logged and shown in output (via `LogTo`)

### Relationships
- [ ] One-to-many is configured with `HasMany` / `WithOne` Fluent API
- [ ] Many-to-many with a payload join entity is implemented
- [ ] `DeleteBehavior.Restrict` is used as the safe default; cascade is explicitly opted in
- [ ] `OwnsOne` / `OwnsMany` is used for a value object embedded in an entity
- [ ] Navigation properties use private setters enforced by the aggregate root

## Suggested Questions
1. What is the difference between `Add()` vs `Attach()` vs `Update()` on `DbContext`?
2. When does `AsNoTracking()` not save memory (hint: owned types)?
3. How do you detect an N+1 query problem in EF Core?
4. What is the difference between eager, lazy, and explicit loading?
5. Why should you avoid putting `DbContext` in the Domain layer of Clean Architecture?
