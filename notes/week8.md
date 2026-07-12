# Week 8 - Testing and Performance

## Topics
- **UnitTests** – xUnit, AAA pattern, mocking (Moq), test isolation, FluentAssertions
- **IntegrationTests** – WebApplicationFactory, TestServer, in-memory databases, HTTP client testing
- **Benchmarks** – BenchmarkDotNet, memory diagnosers, comparing implementations

## Notes

### Unit Tests
- `Shared/` is a small class library (net10.0) with an `OrderService` that depends only on
  abstractions (`IOrderRepository`, `IDiscountService`) — the natural seam for mocking.
- `UnitTests/OrderServiceTests.cs` mocks both collaborators with **Moq** and asserts with
  **FluentAssertions**, following AAA and the `MethodName_Scenario_ExpectedOutcome` naming
  convention. Covers: discounted total calculation, verifying `SaveAsync` is called exactly
  once, no-discount path, and two validation-failure paths (empty order, zero quantity) where
  the mock is also verified as **never** called.
- `UnitTests/PercentageDiscountServiceTests.cs` tests the concrete `PercentageDiscountService`
  directly (no mocking needed — it has no dependencies), covering below/at/above the bulk
  discount threshold.
- Each test class gets a fresh set of mocks/SUT via the xUnit constructor — no shared mutable
  state between tests.

### Integration Tests
- `SampleApi/` is a minimal-API project (net10.0) exposing `/products` CRUD backed by EF Core's
  `UseInMemoryDatabase` provider. `Program.cs` ends with `public partial class Program { }` so
  `WebApplicationFactory<Program>` can host it in-process.
- `IntegrationTests/CustomWebApplicationFactory.cs` overrides the `ProductDbContext`
  registration with a uniquely-named in-memory database (`Guid` per factory instance) so test
  runs never leak state into each other.
- `IntegrationTests/ProductsApiTests.cs` uses `factory.CreateClient()` and exercises the full
  pipeline (routing, model binding, DI, EF Core) over real HTTP: POST/Create, GET by id
  (found + 404), GET all, PUT update, DELETE + subsequent 404.

### Benchmarks
- `Benchmarks/StringConcatenationBenchmarks.cs` compares `+=` string concatenation vs
  `StringBuilder.Append` across `[Params(100, 1_000, 10_000)]` iterations, `[MemoryDiagnoser]`
  on. Confirms the expected O(n²) allocation blow-up of naive concatenation: at 10,000
  iterations, `StringBuilder` allocates ~1,900x less than concatenation (53 KB vs ~100 MB) and
  runs ~12x faster.
- `Benchmarks/OrderTotalBenchmarks.cs` compares LINQ `Sum` vs a manual `for` loop for computing
  an order's subtotal across `[Params(10, 100, 1_000)]` item counts, using `[GlobalSetup]` to
  build the order once per parameter set.
- Run with `dotnet run -c Release` from `Benchmarks/` (always Release — Debug numbers are not
  trustworthy for BenchmarkDotNet).

## Coverage Audit

| Topic | Covered? | File |
|---|---|---|
| xUnit + AAA + naming convention | ✅ | `UnitTests/OrderServiceTests.cs`, `UnitTests/PercentageDiscountServiceTests.cs` |
| Mocking with Moq | ✅ | `UnitTests/OrderServiceTests.cs` (`Mock<IOrderRepository>`, `Mock<IDiscountService>`) |
| FluentAssertions | ✅ | Both `UnitTests/*.cs` files |
| Test isolation (no shared state) | ✅ | Fresh mocks/SUT per test via xUnit constructor |
| `WebApplicationFactory<TEntryPoint>` | ✅ | `IntegrationTests/CustomWebApplicationFactory.cs` |
| In-memory database for EF Core tests | ✅ | `SampleApi/Data/ProductDbContext.cs` (`UseInMemoryDatabase`), overridden per-factory in `CustomWebApplicationFactory` |
| Full request pipeline test (routing/DI/EF) | ✅ | `IntegrationTests/ProductsApiTests.cs` |
| `[MemoryDiagnoser]` | ✅ | Both `Benchmarks/*.cs` files |
| Release-mode benchmarking | ✅ | Verified via `dotnet run -c Release` |
| `[Params(...)]` across input sizes | ✅ | Both `Benchmarks/*.cs` files |
| `[GlobalSetup]` | ✅ | `Benchmarks/OrderTotalBenchmarks.cs` |
| Two implementations compared (Mean + Allocated) | ✅ | `StringConcatenationBenchmarks` (concat vs `StringBuilder`), `OrderTotalBenchmarks` (LINQ vs `for`) |

### Gaps to Address
- None — all Week 8 topics implemented. All five projects (`Shared`, `SampleApi`, `UnitTests`,
  `IntegrationTests`, `Benchmarks`) target `net10.0`; full solution build is 0 warnings/errors,
  all 14 tests pass, and the benchmark suite was smoke-tested end-to-end in Release mode.

