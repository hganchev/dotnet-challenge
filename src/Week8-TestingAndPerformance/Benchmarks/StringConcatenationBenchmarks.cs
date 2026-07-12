using BenchmarkDotNet.Attributes;

namespace Benchmarks;

/// <summary>
/// Compares naive `+=` string concatenation against <see cref="StringBuilder"/> across
/// increasing iteration counts. Demonstrates the O(n^2) allocation cost of repeated string
/// concatenation versus StringBuilder's amortised O(n) buffer growth.
/// </summary>
[MemoryDiagnoser]
public class StringConcatenationBenchmarks
{
    [Params(100, 1_000, 10_000)]
    public int Iterations { get; set; }

    [Benchmark(Baseline = true)]
    public string StringConcatenation()
    {
        var result = string.Empty;
        for (var i = 0; i < Iterations; i++)
        {
            result += "x";
        }

        return result;
    }

    [Benchmark]
    public string StringBuilderAppend()
    {
        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < Iterations; i++)
        {
            builder.Append('x');
        }

        return builder.ToString();
    }
}
