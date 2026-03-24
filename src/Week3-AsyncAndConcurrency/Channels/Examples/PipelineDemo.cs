using System.Threading.Channels;
using Channels.Concepts;

namespace Channels.Examples;

/// <summary>
/// Two-stage and three-stage channel pipelines.
/// Stage 1: parse raw strings into integers.
/// Stage 2: square each integer.
/// Stage 3 (extended): format the result for output.
/// Uses <see cref="ChannelStage{TIn,TOut}"/> for clean composition.
/// </summary>
public static class PipelineDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Pipeline Demo ===\n");

        await TwoStagePipelineAsync();
        await ThreeStagePipelineAsync();
    }

    // ------------------------------------------------------------------ //
    //  1. Two-stage pipeline using ChannelStage<TIn,TOut>
    // ------------------------------------------------------------------ //
    private static async Task TwoStagePipelineAsync()
    {
        Console.WriteLine("--- Two-stage pipeline (parse → square) ---");

        // Raw input strings
        var rawChannel    = Channel.CreateBounded<string>(4);
        // Parsed integers
        var parsedChannel = Channel.CreateBounded<int>(4);
        // Squared results — must be consumed concurrently to avoid deadlock
        var resultChannel = Channel.CreateBounded<long>(4);

        // Stage 1: string → int
        var stage1 = new ChannelStage<string, int>(
            rawChannel.Reader, parsedChannel.Writer,
            (s, _) => ValueTask.FromResult(int.Parse(s)),
            name: "Parse");

        // Stage 2: int → long (squared)
        var stage2 = new ChannelStage<int, long>(
            parsedChannel.Reader, resultChannel.Writer,
            (n, _) => ValueTask.FromResult((long)n * n),
            name: "Square");

        // Start both stages concurrently
        var stage1Task = stage1.RunAsync();
        var stage2Task = stage2.RunAsync();

        // ✅ Collect results concurrently — if we waited until after WhenAll the
        //    bounded result channel would fill up and stage 2 would dead-lock.
        var collectTask = Task.Run(async () =>
        {
            var results = new List<long>();
            await foreach (var r in resultChannel.Reader.ReadAllAsync().ConfigureAwait(false))
                results.Add(r);
            return results;
        });

        // Feed the pipeline
        foreach (var s in new[] { "2", "3", "5", "7", "11", "13" })
        {
            await rawChannel.Writer.WriteAsync(s).ConfigureAwait(false);
            Console.WriteLine($"  [Source] pushed '{s}'");
        }
        rawChannel.Writer.Complete();

        await Task.WhenAll(stage1Task, stage2Task).ConfigureAwait(false);
        var results = await collectTask.ConfigureAwait(false);

        Console.Write("  Results: ");
        foreach (var r in results) Console.Write($"{r} ");
        Console.WriteLine("\n");
    }

    // ------------------------------------------------------------------ //
    //  2. Three-stage pipeline with simulated latency per stage
    // ------------------------------------------------------------------ //
    private static async Task ThreeStagePipelineAsync()
    {
        Console.WriteLine("--- Three-stage pipeline (fetch → transform → format) ---");

        var fetchCh     = Channel.CreateBounded<int>(3);
        var transformCh = Channel.CreateBounded<double>(3);
        var formatCh    = Channel.CreateBounded<string>(3);

        // Stage 1: fetch — simulate I/O delay, double the value
        var stage1 = new ChannelStage<int, double>(
            fetchCh.Reader, transformCh.Writer,
            async (id, ct) =>
            {
                await Task.Delay(10, ct).ConfigureAwait(false);   // simulate I/O
                return id * 1.5;
            },
            name: "Fetch");

        // Stage 2: transform — CPU work (square root)
        var stage2 = new ChannelStage<double, string>(
            transformCh.Reader, formatCh.Writer,
            async (val, ct) =>
            {
                await Task.Delay(5, ct).ConfigureAwait(false);    // simulate light work
                return $"result({val:F2})→{Math.Sqrt(val):F4}";
            },
            name: "Transform");

        var stage1Task = stage1.RunAsync();
        var stage2Task = stage2.RunAsync();

        // ✅ Collect results concurrently to prevent bounded formatCh from filling up
        var collectTask = Task.Run(async () =>
        {
            var items = new List<string>();
            await foreach (var s in formatCh.Reader.ReadAllAsync().ConfigureAwait(false))
                items.Add(s);
            return items;
        });

        // Source
        for (var i = 1; i <= 6; i++)
            await fetchCh.Writer.WriteAsync(i).ConfigureAwait(false);
        fetchCh.Writer.Complete();

        await Task.WhenAll(stage1Task, stage2Task).ConfigureAwait(false);
        var formatted = await collectTask.ConfigureAwait(false);

        Console.WriteLine("  Formatted output:");
        foreach (var s in formatted) Console.WriteLine($"    {s}");

        Console.WriteLine();
    }
}
