---
applyTo: "src/Week8-TestingAndPerformance/**"
---
# Week 8 — Testing and Performance

## Unit Tests (xUnit)
- Follow **AAA**: Arrange (set up), Act (call the SUT), Assert (verify outcome) — one logical assertion per test.
- Name tests with the pattern `MethodName_Scenario_ExpectedOutcome` (e.g., `Calculate_NegativeInput_ThrowsArgumentException`).
- Each test must be independent — no shared mutable state between tests; use xUnit constructors for per-test setup.
- Use `Moq` or `NSubstitute` to mock dependencies; never mock the system-under-test itself.
- Prefer `FluentAssertions` (`result.Should().Be(42)`) over raw `Assert` calls for readable failure messages.
- Aim for tests that target behaviour, not implementation details — prefer black-box testing of public APIs.

## Integration Tests
- Use `WebApplicationFactory<TEntryPoint>` to spin up a real in-memory ASP.NET Core server for HTTP-level tests.
- Use an in-memory SQLite database (or `UseInMemoryDatabase`) instead of a real database for EF Core integration tests.
- Isolate each test class with a fresh `WebApplicationFactory` scope to prevent state leakage between test runs.
- Test the full request pipeline: middleware, auth, routing, DI resolution — not just handler logic in isolation.
- Use `HttpClient` obtained from `factory.CreateClient()` to send requests; verify status codes and response bodies.

## Benchmarks (BenchmarkDotNet)
- Always decorate benchmark classes with `[MemoryDiagnoser]` to track allocations alongside throughput.
- Run benchmarks in **Release** mode: `dotnet run -c Release`. Never trust Debug-mode numbers.
- Use `[Params(...)]` to compare across different input sizes or configurations in a single benchmark run.
- Use `[GlobalSetup]` for expensive one-time prep; use `[IterationSetup]` only when state must reset each iteration.
- Compare at least two implementations side-by-side; look at both `Mean` time and `Allocated` bytes.
- Avoid micro-optimisations that harm readability without a clear, measured performance gain from a benchmark.
