using ObservabilityAndTracing.Examples;

await StructuredLoggingDemo.RunAsync();
Console.WriteLine();
await DistributedTracingDemo.RunAsync();
Console.WriteLine();
await HealthChecksDemo.RunAsync();
