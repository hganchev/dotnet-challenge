---
applyTo: "src/Week7-DesignPatterns/**"
---
# Week 7 — Design Patterns

## SOLID Principles
- **SRP**: if a class has more than one reason to change, split it. Services, repositories, and validators are separate types.
- **OCP**: extend behaviour by adding new types, not by editing existing ones. Use polymorphism and dependency injection.
- **LSP**: subtypes must not weaken preconditions, strengthen postconditions, or throw exceptions the base type does not declare.
- **ISP**: prefer multiple small, focused interfaces over one fat interface. Consumers depend only on what they call.
- **DIP**: high-level modules depend on abstractions (interfaces). Inject dependencies via constructor — no `new ConcreteType()` in business logic.

## Creational Patterns
- Implement `Singleton` with `Lazy<T>`: `private static readonly Lazy<T> _instance = new(() => new T());`.
- Use `Factory Method` to decouple object creation from the caller; return interface types, not concrete types.
- `Builder` should return `this` from each setter method (fluent API); call `Build()` / `Create()` at the end for the final object.

## Structural Patterns
- `Decorator` wraps an interface, not a concrete class — add behaviour without touching the original implementation.
- Stack decorators by registering multiple wrappers in DI (outermost registered last).

## Behavioural Patterns
- `Strategy` encapsulates a family of algorithms behind an interface; inject the strategy via constructor or resolve from DI.
- `Observer` with C# events: unsubscribe (`-=`) to prevent memory leaks when the subscriber outlives the publisher.
- Prefer `IObservable<T>` / `IObserver<T>` (Rx pattern) over raw events for complex, composable event streams.

## CQRS & Repository
- `ICommand` / `ICommandHandler<TCommand>` for writes; `IQuery<TResult>` / `IQueryHandler<TQuery, TResult>` for reads.
- `Dispatcher` resolves handlers from `IServiceProvider` — no hard-coded handler references, no MediatR required.
- `Repository` abstracts the data store behind a collection-like interface; the domain layer depends only on the interface (DIP).
- Do not let repository interfaces leak `IQueryable<T>` into the domain layer — keep the domain store-agnostic.

## Clean Architecture
- Dependencies point **inward only**: Presentation → Application → Domain; Infrastructure → Application + Domain.
- Domain layer has zero external dependencies (no EF, no HTTP, no DI container).
- Application layer defines ports (interfaces); Infrastructure provides adapters (concrete implementations).
- Map between layers at the boundary — domain entities must not leak into API responses.
