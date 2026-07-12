using BenchmarkDotNet.Running;
using Benchmarks;

// Always run benchmarks in Release mode: `dotnet run -c Release`.
BenchmarkSwitcher
    .FromTypes([typeof(StringConcatenationBenchmarks), typeof(OrderTotalBenchmarks)])
    .Run(args);
