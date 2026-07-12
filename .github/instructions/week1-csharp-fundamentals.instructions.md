---
applyTo: "src/Week1-CSharpFundamentals/**"
---
# Week 1 — C# Fundamentals

## Value vs Reference Types
- Use `struct` only for small (≤ 16 bytes), immutable, value-semantic types (e.g. `Point`, `Color`).
- Always implement `IEquatable<T>` when overriding `Equals()` on a struct to avoid boxing.
- Avoid boxing; prefer generic collections (`List<T>`) instead of non-generic (`ArrayList`).
- Use `Nullable<T>` / `T?` for value types that can be absent; check with `is null` or `.HasValue`.
- Use `ref` returns and `ref` locals only when measurable performance gains justify the complexity.

## OOP
- Prefer composition over inheritance for behavioural reuse.
- Use `abstract class` when sharing implementation; use `interface` for contracts with no shared state.
- Mark leaf classes `sealed` unless explicitly designed for inheritance.
- Use `record` / `record struct` for DTOs and value objects that need structural equality.
- Keep constructors simple — do not perform heavy work or I/O in constructors.
- Prefer `init`-only setters over settable properties to communicate post-construction immutability.
- Default to `private` access and widen (`internal` → `public`) only when a member is part of the intended contract.
- Use `static` members only for stateless helpers or true singletons; avoid static mutable state (see `StaticMembers`).
- Overload operators (`==`, `+`, etc.) only on value-like types where the operation has an unambiguous meaning, and always pair `==`/`!=` with `Equals`/`GetHashCode` overrides.

## Exceptions
- Catch only exception types you can meaningfully handle; never catch bare `Exception` except at a top-level/global boundary.
- Derive custom exceptions from `Exception` (not `ApplicationException`) and add context via constructor parameters/properties, not string parsing.
- Rethrow with `throw;` (never `throw ex;`) to preserve the original stack trace.
- Use exception filters (`catch (IOException ex) when (...)`) to inspect an exception without a catch-then-rethrow.
- Reserve exceptions for exceptional conditions — do not use them for normal/expected control flow (e.g. validation failures).

## Delegates & Events
- Prefer `Func<>` / `Action<>` over custom `delegate` declarations unless a named type improves clarity.
- Raise events safely with the null-conditional operator: `EventName?.Invoke(this, args)`.
- Follow the `EventHandler<TEventArgs>` pattern for public events; derive payload from `EventArgs`.
- Unsubscribe event handlers (`-=`) when the subscriber's lifetime is shorter than the publisher's to prevent memory leaks.
- Prefer lambdas for short inline callbacks; use named method groups when the callback has a reusable identity.

## Generics
- Apply type constraints (`where T : class`, `where T : IComparable<T>`, `where T : new()`) to make intent explicit.
- Use covariant (`out T`) interfaces for read-only/producer abstractions; contravariant (`in T`) for write-only/consumer abstractions.
- Avoid using `object` parameters where a generic type parameter can be applied — prevents boxing and aids type safety.
- Prefer `default` literal over `default(T)` in C# 7.1+ contexts.
