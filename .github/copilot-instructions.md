# DotNet Challenge — Global Copilot Instructions

This is a 10-week guided .NET learning challenge. Each week lives under `src/WeekN-<Topic>/` with
sub-projects per concept. Console demos go in `Program.cs`; supporting types in named sub-folders.

## Language & Runtime
- Target **.NET 10** / **C# 14** unless a week explicitly calls for something else.
- Enable nullable reference types (`<Nullable>enable</Nullable>`) in every project.
- Use `global using` directives only when they genuinely reduce repetition across multiple files.

## Naming & Style
- **PascalCase**: types, methods, properties, events, constants.
- **camelCase with `_` prefix**: private instance fields (`_myField`).
- **camelCase**: local variables and parameters.
- Prefer `var` when the right-hand side makes the type obvious; spell it out otherwise.
- Prefer expression-bodied members for single-expression methods and properties.
- Use `is null` / `is not null` instead of `== null`.
- Prefer pattern matching and `switch` expressions over long `if/else if` chains.

## Code Quality
- No unused `using` directives.
- No magic numbers — use named constants or `enum` values.
- Keep methods short and focused on a single responsibility.
- Prefer `readonly` fields and `init`-only setters to communicate immutability.
- Avoid `static` mutable state except where explicitly demonstrating the Singleton pattern.

## Demo / Learning Code
- Each `Program.cs` should produce readable console output that demonstrates the concept.
- Add a brief XML doc comment (`/// <summary>`) to every public type and method that isn't
  self-explanatory from its name.
- Leave `// TODO:` markers for topics that are not yet implemented (see week coverage audits).

## Project Structure
| Week | Folder | Topic |
|---|---|---|
| 1 | `Week1-CSharpFundamentals` | Value/Reference types, OOP, Delegates, Generics |
| 2 | `Week2-MemoryAndRuntime` | GC, IDisposable, Span\<T\>, CLR internals |
| 3 | `Week3-AsyncAndConcurrency` | async/await, TPL, Thread safety, Channels |
| 4 | `Week4-LinqAndCollections` | LINQ internals, Collections, Performance |
| 5 | `Week5-AspNetCore` | Middleware, DI, Minimal APIs, Auth |
| 6 | `Week6-EFCore` | Fundamentals, Query optimisation, Relationships |
| 7 | `Week7-DesignPatterns` | SOLID, Common patterns, Clean Architecture |
| 8 | `Week8-TestingAndPerformance` | Unit tests, Integration tests, Benchmarks |
| 9 | `Week9-SystemDesign` | Scalability/Caching, Messaging, Resilience/Rate limiting, Observability |
| 10 | `Week10-AdvancedSystemDesign` | Distributed data/Consensus, API gateway, Database replication, Security at scale, Real-time/Stream processing, Capstone |
