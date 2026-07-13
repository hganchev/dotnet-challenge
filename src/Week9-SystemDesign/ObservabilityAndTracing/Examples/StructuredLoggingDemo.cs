using ObservabilityAndTracing.Logging;
using Serilog;

namespace ObservabilityAndTracing.Examples;

/// <summary>Demonstrates structured logging with Serilog and a correlation ID enriched onto every
/// log line of a multi-step async flow.</summary>
public static class StructuredLoggingDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Structured Logging with Serilog + Correlation ID ===");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "  [{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}")
            .CreateLogger();

        try
        {
            var flow = new OrderRequestFlow(Log.Logger);
            await flow.ProcessAsync("ORD-9001");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
