using System.Threading.Channels;
using ObservabilityAndTracing.Tracing;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace ObservabilityAndTracing.Examples;

/// <summary>Demonstrates OpenTelemetry tracing: a root span for the overall flow, and a child span
/// created inside a background channel consumer — an async boundary beyond the original method
/// call — to show trace context propagates across it.</summary>
public static class DistributedTracingDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Distributed Tracing with OpenTelemetry ===");

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(OrderProcessingTracing.ActivitySource.Name)
            .AddConsoleExporter()
            .Build();

        using var rootActivity = OrderProcessingTracing.ActivitySource.StartActivity("ProcessOrder");
        rootActivity?.SetTag("order.id", "ORD-9001");

        var channel = Channel.CreateUnbounded<string>();
        var consumerTask = ConsumeAsync(channel.Reader);

        await channel.Writer.WriteAsync("charge-payment");
        channel.Writer.Complete();
        await consumerTask;
    }

    // Runs on a separate async continuation after `ReadAllAsync` suspends and resumes —
    // `Activity.Current` still flows here because .NET propagates it via AsyncLocal<T>
    // across await boundaries automatically, without any manual context passing.
    private static async Task ConsumeAsync(ChannelReader<string> reader)
    {
        await foreach (var message in reader.ReadAllAsync())
        {
            using var childActivity = OrderProcessingTracing.ActivitySource.StartActivity("HandleChannelMessage");
            childActivity?.SetTag("message", message);
            await Task.Delay(10);
            Console.WriteLine($"  [Consumer] handled '{message}' (trace {childActivity?.TraceId})");
        }
    }
}
