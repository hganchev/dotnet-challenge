using System.Diagnostics;

namespace ObservabilityAndTracing.Tracing;

/// <summary>The single <see cref="ActivitySource"/> this demo uses to create spans. In a real app
/// there'd typically be one per logical component/service.</summary>
public static class OrderProcessingTracing
{
    public static readonly ActivitySource ActivitySource = new("Week9.ObservabilityAndTracing");
}
