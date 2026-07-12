# Week 7 - Design Patterns

## Goal
Understand and apply widely-used software design patterns and architectural principles in .NET: how SOLID guides class design, how creational/structural/behavioural patterns solve recurring problems, and how Clean Architecture organises an entire solution around the domain.

---

## 1. SOLID Principles

### Topics to Cover
| Principle | Key Points |
|---|---|
| Single Responsibility (SRP) | One reason to change per class; separate concerns into distinct types |
| Open/Closed (OCP) | Open for extension, closed for modification; use abstractions + polymorphism |
| Liskov Substitution (LSP) | Subtypes must be substitutable for their base type without breaking behaviour |
| Interface Segregation (ISP) | Prefer many small, focused interfaces over one fat interface |
| Dependency Inversion (DIP) | High-level modules depend on abstractions; inject dependencies via constructor |

### Expected Outcomes
- Identify SRP violations and refactor them into separate classes
- Demonstrate OCP by adding new behaviour without editing existing classes
- Explain why an LSP violation breaks polymorphic code and how to fix it
- Show how ISP avoids forcing empty interface implementations on consumers
- Wire dependencies via constructor injection and explain why DIP enables testability

---

## 2. Common Patterns

### Topics to Cover
| Pattern | Category | Key Points |
|---|---|---|
| Singleton | Creational | Single instance; thread-safe `Lazy<T>` implementation; when to avoid it |
| Factory Method | Creational | Delegate object creation to a factory method; decouple caller from concrete type |
| Builder | Creational | Construct complex objects step by step; fluent API with method chaining |
| Strategy | Behavioural | Encapsulate interchangeable algorithms behind an interface; swap at runtime |
| Observer | Behavioural | Publisher/subscriber; .NET `event` keyword; `IObservable<T>` / `IObserver<T>` |
| Decorator | Structural | Wrap an object to add behaviour without inheritance; open for extension |
| Repository | Data Access | Abstract data storage behind a collection-like interface; testable persistence |
| CQRS | Architectural | Separate write model (commands) from read model (queries); no shared handler dispatch |

### Expected Outcomes
- Implement a thread-safe Singleton using `Lazy<T>`
- Build a fluent Builder for a multi-field object
- Swap Strategy implementations at runtime without touching the caller
- Implement Observer with C# events and explain memory-leak risks
- Stack Decorators without changing the decorated type
- Design a generic Repository and explain how it supports the DIP
- Implement CQRS with `ICommand` / `IQuery<TResult>` and typed handlers linked via a generic `Dispatcher` (no MediatR)

---

## 3. Clean Architecture

### Topics to Cover
| Layer | Responsibility | Allowed Dependencies |
|---|---|---|
| Domain | Entities, value objects, domain events, business rules | None (innermost) |
| Application | Use-cases, commands/queries, ports (interfaces) | Domain only |
| Infrastructure | EF Core, HTTP clients, file system, repos, message brokers | Application + Domain |
| Presentation | Controllers, Minimal API endpoints, CLI, Blazor pages | Application only |

| Concept | Key Points |
|---|---|
| Dependency rule | Dependencies point inward; outer layers depend on inner, never the reverse |
| Ports & Adapters | Application defines interfaces (ports); Infrastructure provides concrete adapters |
| Use-case classes | One class per use-case; orchestrates domain logic, calls repository ports |
| Mapping | DTOs at the boundary; domain objects never leak to Presentation |
| Testability | Domain + Application fully unit-testable; Infrastructure mocked via interfaces |

### Expected Outcomes
- Draw the four-layer diagram and explain which references are illegal
- Create a use-case class that takes a command, validates it, calls a domain entity, and persists via a repository interface
- Explain why putting EF Core `DbContext` in the Domain layer violates the dependency rule
- Describe how CQRS maps onto Clean Architecture (commands and queries live in Application)

---

## 4. Vertical Slice Architecture

### Topics to Cover
| Concept | Key Points |
|---|---|
| Feature folder | Each use-case (e.g. `CreateOrder`, `GetOrderById`) owns its own request, handler, and response in one folder/file |
| Vertical vs. horizontal slicing | Slice by feature, not by technical layer (no separate `Controllers/`, `Services/`, `Repositories/` for every feature) |
| Coupling | Minimize coupling *between* slices; a change to one feature should not ripple into unrelated features |
| Shared kernel | Cross-cutting concerns (validation pipeline, logging, persistence infrastructure) live in a small shared module referenced by every slice |
| Relationship to CQRS | Each slice is typically a single command or query + handler, so CQRS and Vertical Slice Architecture compose naturally |

### Expected Outcomes
- Explain why Vertical Slice Architecture reduces the "shotgun surgery" problem common in layered architectures
- Implement at least two feature slices (one command, one query) as fully self-contained folders
- Identify what belongs in a shared kernel versus what should stay duplicated per-slice
- Compare Vertical Slice Architecture with Clean Architecture and describe when to prefer each

---

## Coverage Audit

### SOLID
| Principle | Covered? | File |
|---|---|---|
| SRP | ⬜ | `SOLID/` – not yet implemented |
| OCP | ⬜ | `SOLID/` – not yet implemented |
| LSP | ⬜ | `SOLID/` – not yet implemented |
| ISP | ⬜ | `SOLID/` – not yet implemented |
| DIP | ⬜ | `SOLID/` – not yet implemented |

### Common Patterns
| Pattern | Covered? | File |
|---|---|---|
| Singleton | ⬜ | `CommonPatterns/` – not yet implemented |
| Factory Method | ⬜ | `CommonPatterns/` – not yet implemented |
| Builder | ⬜ | `CommonPatterns/` – not yet implemented |
| Strategy | ⬜ | `CommonPatterns/` – not yet implemented |
| Observer | ⬜ | `CommonPatterns/` – not yet implemented |
| Decorator | ⬜ | `CommonPatterns/` – not yet implemented |
| Repository | ⬜ | `CommonPatterns/` – not yet implemented |
| CQRS – `ICommand` / `ICommandHandler<T>` | ✅ | `CommonPatterns/CQRS/Abstractions/ICommand.cs`, `ICommandHandler.cs` |
| CQRS – `IQuery<TResult>` / `IQueryHandler<TQuery,TResult>` | ✅ | `CommonPatterns/CQRS/Abstractions/IQuery.cs`, `IQueryHandler.cs` |
| CQRS – `IDispatcher` + `Dispatcher` (generic, no MediatR) | ✅ | `CommonPatterns/CQRS/Abstractions/IDispatcher.cs`, `CQRS/Dispatcher.cs` |
| CQRS – `CreateOrderCommand` + handler | ✅ | `CommonPatterns/CQRS/Commands/CreateOrderCommand.cs`, `CreateOrderCommandHandler.cs` |
| CQRS – `GetOrderByIdQuery` + handler | ✅ | `CommonPatterns/CQRS/Queries/GetOrderByIdQuery.cs`, `GetOrderByIdQueryHandler.cs` |
| CQRS – `GetAllOrdersQuery` + handler | ✅ | `CommonPatterns/CQRS/Queries/GetAllOrdersQuery.cs`, `GetAllOrdersQueryHandler.cs` |
| CQRS – In-memory repository + domain entity | ✅ | `CommonPatterns/CQRS/Infrastructure/`, `CQRS/Domain/Order.cs` |

### Clean Architecture
| Topic | Covered? | File |
|---|---|---|
| Layer separation | ⬜ | `CleanArchitecture/` – not yet implemented |
| Domain entities | ⬜ | `CleanArchitecture/` – not yet implemented |
| Use-case classes | ⬜ | `CleanArchitecture/` – not yet implemented |
| Repository interface (port) | ⬜ | `CleanArchitecture/` – not yet implemented |
| Infrastructure adapter | ⬜ | `CleanArchitecture/` – not yet implemented |
| Presentation entry point | ⬜ | `CleanArchitecture/` – not yet implemented |

### Vertical Slice Architecture
| Topic | Covered? | File |
|---|---|---|
| Project scaffold | ✅ | `VerticalSlices/VerticalSlices.csproj` |
| Feature folders (request/handler/response per use-case) | ⬜ | `VerticalSlices/` – not yet implemented |
| Shared kernel / cross-cutting infrastructure | ⬜ | `VerticalSlices/` – not yet implemented |

### Gaps to Address
- Implement all five SOLID principles in `SOLID/`
- Add remaining CommonPatterns: Singleton, Factory, Builder, Strategy, Observer, Decorator, Repository
- Implement the CleanArchitecture demo with all four layers wired together
- Implement the VerticalSlices demo with at least two feature folders and a shared kernel

