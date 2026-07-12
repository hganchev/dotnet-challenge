# .NET Challenge

A 10-week deep-dive into .NET and C# covering fundamentals, runtime internals, concurrency, data access, web APIs, design patterns, testing, and system design.

## Structure

```
src/
├── Week1-CSharpFundamentals/
│   ├── ValueVsReference/       # Console — value vs reference types, boxing
│   ├── OOP/                    # Console — classes, interfaces, inheritance
│   ├── DelegatesAndEvents/     # Console — delegates, events, Func/Action
│   └── Generics/               # Console — generic types, constraints, variance
├── Week2-MemoryAndRuntime/
│   ├── GarbageCollector/       # Console — GC generations, finalization
│   ├── IDisposablePattern/     # Console — IDisposable, using, SafeHandle
│   ├── SpanAndMemory/          # Console — Span<T>, Memory<T>, stackalloc
│   └── ClrInternals/           # Console — JIT, type system, CLR internals
├── Week3-AsyncAndConcurrency/
│   ├── AsyncAwait/             # Console — async/await, Task, ValueTask
│   ├── TPL/                    # Console — Parallel, PLINQ, Dataflow
│   ├── ThreadSafety/           # Console — lock, Interlocked, concurrent collections
│   └── Channels/               # Console — System.Threading.Channels
├── Week4-LinqAndCollections/
│   ├── LinqInternals/          # Console — deferred execution, expression trees
│   ├── Collections/            # Console — Dictionary, HashSet, sorted collections
│   └── LinqPerformance/        # Console — enumeration pitfalls, streaming
├── Week5-AspNetCore/
│   ├── Middleware/             # Web API — pipeline, request delegates
│   ├── DependencyInjection/    # Web API — DI lifetimes, IServiceProvider
│   ├── MinimalApis/            # Web API — minimal endpoints, route groups
│   └── Auth/                   # Web API — JWT, policies, claims
├── Week6-EFCore/
│   ├── Fundamentals/           # Console — DbContext, migrations, Fluent API
│   ├── QueryOptimization/      # Console — AsNoTracking, eager/lazy loading
│   └── Relationships/          # Console — one-to-many, many-to-many
├── Week7-DesignPatterns/
│   ├── SOLID/                  # Console — SOLID principles with examples
│   ├── CommonPatterns/         # Console — Factory, Strategy, Observer, etc.
│   └── CleanArchitecture/      # Console — layered architecture demo
├── Week8-TestingAndPerformance/
│   ├── UnitTests/              # xUnit — unit testing with mocks
│   ├── IntegrationTests/       # xUnit — WebApplicationFactory, TestServer
│   └── Benchmarks/             # Console — BenchmarkDotNet micro-benchmarks
├── Week9-SystemDesign/
│   ├── ScalabilityAndCaching/              # Console — load balancing, IMemoryCache/IDistributedCache
│   ├── MessagingAndEventDrivenArchitecture/ # Console — pub/sub, outbox pattern, event sourcing basics
│   ├── ResilienceAndRateLimiting/           # Console — Polly retry/circuit-breaker, rate limiters
│   └── ObservabilityAndTracing/             # Web API — Serilog, OpenTelemetry, health checks
└── Week10-AdvancedSystemDesign/
    ├── DistributedDataAndConsistency/       # Console — CAP theorem, sharding, consensus basics
    ├── ApiGatewayAndServiceCommunication/   # Console — REST vs gRPC, API gateway, service discovery
    ├── DatabaseDesignAndReplication/        # Console — SQL vs NoSQL, indexing, replication, consistent hashing
    ├── SecurityAtScale/                     # Console — cross-service auth, encryption, secrets management
    ├── RealTimeAndStreamProcessing/         # Console — WebSockets/SSE, batch vs stream processing
    └── CapstoneAdvancedSystemDesign/        # Design doc + partial impl — end-to-end case study
notes/
├── week1.md  …  week10.md      # Study notes per week
```

## Getting Started

```bash
# Build entire solution
dotnet build DotNetChallenge.sln

# Run a specific project
dotnet run --project src/Week1-CSharpFundamentals/OOP

# Run tests
dotnet test src/Week8-TestingAndPerformance/UnitTests
dotnet test src/Week8-TestingAndPerformance/IntegrationTests

# Run benchmarks (Release mode required)
dotnet run --project src/Week8-TestingAndPerformance/Benchmarks -c Release
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
