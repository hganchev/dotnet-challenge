# Week 6 - Entity Framework Core

## Goal
Master EF Core as a code-first ORM: configure a `DbContext` and generate migrations, understand how conventions/data annotations/Fluent API each shape the model, write queries that avoid change-tracking and N+1 overhead, and configure one-to-one, one-to-many, and many-to-many relationships explicitly.

---

## 1. EF Core Fundamentals

> Project: `src/Week6-EFCore/Fundamentals/` — SQLite-backed, `dotnet ef migrations` generated

### Topics to Cover
| Topic | Key Points |
|---|---|
| `DbContext` | Represents a session with the database; exposes `DbSet<T>` properties; **Scoped** lifetime in ASP.NET Core |
| Code-first workflow | Model classes → `DbContext` → `dotnet ef migrations add` → `dotnet ef database update` |
| Conventions | EF infers PK (`Id` / `<Type>Id`), FK, required-ness from nullability, string length from nothing (unbounded) without configuration |
| Data annotations | `[Key]`, `[Required]`, `[MaxLength]`, `[Column]`, `[Table]` — attribute-based configuration on the entity itself |
| Fluent API | `OnModelCreating(ModelBuilder)` — `.HasKey()`, `.Property().HasMaxLength()`, `.ToTable()` — overrides annotations, used for anything annotations can't express |
| Migrations | `Migrations/` folder — `Up()`/`Down()` methods, `__EFMigrationsHistory` table tracks applied migrations |
| `DbContext` lifetime & DI | One `DbContext` instance per unit of work; dispose after use; never share across threads |

### Expected Outcomes
- Explain what EF infers by convention vs what must be configured explicitly
- Configure the same constraint with both a data annotation and the equivalent Fluent API call
- Generate and apply a migration, then inspect the generated `Up()`/`Down()` SQL
- Explain why `DbContext` must be Scoped, not Singleton, in a web app

---

## 2. Query Optimization

> Project: `src/Week6-EFCore/QueryOptimization/` — SQLite-backed, seeded dataset

### Topics to Cover
| Topic | Key Points |
|---|---|
| Change tracking | Default tracking snapshots entity state to detect changes on `SaveChanges()` — costs memory + CPU |
| `AsNoTracking()` | Skips the change tracker for read-only queries — faster, lower memory |
| Eager loading | `Include()` / `ThenInclude()` — loads related data in the same (or a joined) query, avoids N+1 |
| Lazy loading | Navigation property loads on first access — requires proxies, easy to trigger accidental N+1 |
| Explicit loading | `context.Entry(x).Collection(...).Load()` — load related data on demand, on your terms |
| N+1 problem | One query for the parent + one query per child row — the most common EF perf bug |
| `AsSplitQuery()` | Splits a multi-`Include` query into several SQL queries — avoids cartesian explosion |
| Projections (`Select`) | Retrieve only the columns needed — skips loading full entities |
| Compiled queries | `EF.CompileAsyncQuery()` — caches the query plan for hot, frequently executed queries |
| Raw SQL | `FromSqlInterpolated()` / `ExecuteUpdateAsync()` / `ExecuteDeleteAsync()` — bypass LINQ translation or entity materialization for bulk ops |
| Logging generated SQL | `LogTo(Console.WriteLine, LogLevel.Information)` on `DbContextOptionsBuilder` |

### Expected Outcomes
- Demonstrate the N+1 problem, then fix it with `Include()` and again with a projection
- Measure (relatively) the cost difference between tracked and `AsNoTracking()` queries
- Use `AsSplitQuery()` to avoid a cartesian-explosion warning on multiple collection includes
- Write a compiled query and explain when it's worth the extra complexity
- Perform a bulk update with `ExecuteUpdateAsync()` without loading entities into memory

---

## 3. Relationships

> Project: `src/Week6-EFCore/Relationships/` — SQLite-backed

### Topics to Cover
| Topic | Key Points |
|---|---|
| One-to-one | `HasOne().WithOne().HasForeignKey<T>()` — e.g., `Author` ↔ `AuthorProfile` |
| One-to-many | `HasOne().WithMany()` — e.g., `Author` → many `Book`s; most common relationship |
| Many-to-many | Implicit join table (EF Core 5+) or explicit join entity with payload — e.g., `Book` ↔ `Tag` |
| Navigation properties | Reference (`Author Author`) vs collection (`ICollection<Book> Books`) navigations |
| Cascade delete | `DeleteBehavior.Cascade/Restrict/SetNull/NoAction` — configure explicitly, don't rely on convention defaults |
| Explicit join entity | Needed when the many-to-many relationship itself has data (e.g., `BookTag.AddedAt`) |
| Owned types | `OwnsOne()` for value objects embedded in the owner's table (no separate `DbSet<T>`) |

### Expected Outcomes
- Configure a one-to-one, one-to-many, and many-to-many relationship explicitly via Fluent API
- Explain the difference between an implicit many-to-many join table and an explicit join entity with a payload
- Choose the correct `DeleteBehavior` for a given relationship and justify it
- Model a value object with `OwnsOne()` instead of a separate entity/table

---

## Implementation Plan

### Implementation Order
| Step | Project | Why |
|---|---|---|
| 1 | Fundamentals | Foundation — `DbContext`, conventions, annotations, Fluent API, migrations |
| 2 | Relationships | Build on Fundamentals' model with one-to-one/one-to-many/many-to-many |
| 3 | QueryOptimization | Apply performance techniques against a realistic relational model |

### 1. Fundamentals

**Folder structure:**
```
Fundamentals/
  Program.cs                       # Orchestrator — calls all demos
  Data/
    AppDbContext.cs                 # DbSet<Author>, DbSet<Book>; OnModelCreating (Fluent API)
  Entities/
    Author.cs                      # Data-annotation driven (Required, MaxLength)
    Book.cs                        # Convention + Fluent API driven
  Migrations/                      # dotnet ef migrations add InitialCreate
  Concepts/
    ConventionsDemo.cs             # What EF infers without any configuration
    DataAnnotationsDemo.cs         # [Required]/[MaxLength]/[Table] in action
    FluentApiDemo.cs               # Equivalent Fluent API configuration
    MigrationsDemo.cs              # Apply migration, show __EFMigrationsHistory
```

### 2. Relationships

**Folder structure:**
```
Relationships/
  Program.cs                       # Orchestrator — calls all demos
  Data/
    LibraryDbContext.cs             # OnModelCreating wires all relationship types
  Entities/
    Author.cs / AuthorProfile.cs    # One-to-one
    Book.cs                        # One-to-many (Author -> Books)
    Tag.cs / BookTag.cs             # Many-to-many via explicit join entity (payload: AddedAt)
  Concepts/
    OneToOneDemo.cs
    OneToManyDemo.cs
    ManyToManyDemo.cs
    CascadeDeleteDemo.cs
    OwnedTypeDemo.cs                # Address as an owned value object on AuthorProfile
```

### 3. QueryOptimization

**Folder structure:**
```
QueryOptimization/
  Program.cs                       # Orchestrator — calls all demos
  Data/
    ShopDbContext.cs                # DbSet<Customer>, DbSet<Order>, DbSet<OrderItem>; LogTo wired up
  Entities/
    Customer.cs / Order.cs / OrderItem.cs
  Concepts/
    NPlusOneDemo.cs                 # Show the bug, then fix with Include() and Select()
    NoTrackingDemo.cs               # AsNoTracking() vs tracked query comparison
    EagerVsExplicitLoadingDemo.cs   # Include/ThenInclude vs Entry().Collection().Load()
    SplitQueryDemo.cs               # AsSplitQuery() on multiple collection includes
    CompiledQueryDemo.cs            # EF.CompileAsyncQuery for a hot path
    RawSqlAndBulkOpsDemo.cs         # FromSqlInterpolated, ExecuteUpdateAsync, ExecuteDeleteAsync
```

---

## Notes

### EF Core Fundamentals
- All three projects target **.NET 10** (`net10.0`) with EF Core **10.0.9**; `SQLitePCLRaw.bundle_e_sqlite3` is pinned to `3.0.3` to avoid the transitive `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 high-severity SQLite advisory (GHSA-2m69-gcr7-jv3q / CVE-2025-6965).
- `dotnet-ef` global tool (10.0.0) generates real migrations under `Migrations/` for all three projects; each app calls `Database.MigrateAsync()` at startup.

### Query Optimization
- `LogTo(Console.WriteLine, LogLevel.Information)` on `ShopDbContext`'s options makes the generated SQL visible for every demo, including the JOIN vs. per-row SELECTs in the N+1 fix.

### Relationships
- `AuthorProfile` uses its FK (`AuthorId`) as its PK to enforce true 1:1 cardinality; `BookTag` is an explicit join entity (composite key) carrying `AddedAt`, and `Address` is mapped via `OwnsOne()` onto the `AuthorProfiles` table with no separate table of its own.

---

## Coverage Audit

### EF Core Fundamentals
| Topic | Covered? | File |
|---|---|---|
| `DbContext` + `DbSet<T>` | ✅ | `Fundamentals/Data/AppDbContext.cs` |
| Conventions | ✅ | `Fundamentals/Concepts/ConventionsDemo.cs` |
| Data annotations | ✅ | `Fundamentals/Concepts/DataAnnotationsDemo.cs`, `Fundamentals/Entities/Author.cs` |
| Fluent API | ✅ | `Fundamentals/Concepts/FluentApiDemo.cs`, `Fundamentals/Data/AppDbContext.cs` |
| Migrations | ✅ | `Fundamentals/Concepts/MigrationsDemo.cs`, `Fundamentals/Migrations/` |

### Query Optimization
| Topic | Covered? | File |
|---|---|---|
| N+1 problem + fixes | ✅ | `QueryOptimization/Concepts/NPlusOneDemo.cs` |
| `AsNoTracking()` | ✅ | `QueryOptimization/Concepts/NoTrackingDemo.cs` |
| Eager / explicit loading | ✅ | `QueryOptimization/Concepts/EagerVsExplicitLoadingDemo.cs` |
| `AsSplitQuery()` | ✅ | `QueryOptimization/Concepts/SplitQueryDemo.cs` |
| Compiled queries | ✅ | `QueryOptimization/Concepts/CompiledQueryDemo.cs` |
| Raw SQL / bulk `ExecuteUpdateAsync` / `ExecuteDeleteAsync` | ✅ | `QueryOptimization/Concepts/RawSqlAndBulkOpsDemo.cs` |

### Relationships
| Topic | Covered? | File |
|---|---|---|
| One-to-one | ✅ | `Relationships/Concepts/OneToOneDemo.cs`, `Relationships/Data/LibraryDbContext.cs` |
| One-to-many | ✅ | `Relationships/Concepts/OneToManyDemo.cs` |
| Many-to-many (explicit join entity) | ✅ | `Relationships/Concepts/ManyToManyDemo.cs`, `Relationships/Entities/BookTag.cs` |
| Cascade delete behaviors | ✅ | `Relationships/Concepts/CascadeDeleteDemo.cs` |
| Owned types | ✅ | `Relationships/Concepts/OwnedTypeDemo.cs`, `Relationships/Entities/Address.cs` |

### Gaps to Address
- None — all three Week 6 sub-projects are implemented, migrated to .NET 10, and verified to run end-to-end.
