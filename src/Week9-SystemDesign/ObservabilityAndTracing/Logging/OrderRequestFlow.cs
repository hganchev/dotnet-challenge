using Serilog;
using Serilog.Context;

namespace ObservabilityAndTracing.Logging;

/// <summary>A small multi-step request flow used to demonstrate a correlation ID that stays
/// attached to every log line as it flows across async boundaries.</summary>
public class OrderRequestFlow
{
    private readonly ILogger _logger;

    public OrderRequestFlow(ILogger logger) => _logger = logger;

    public async Task ProcessAsync(string orderId)
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // LogContext properties flow through AsyncLocal<T>, so every awaited call below
        // (even after crossing a real async boundary) still logs the same CorrelationId.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.Information("Received order {OrderId}", orderId);
            await ValidateAsync(orderId);
            await ChargePaymentAsync(orderId);
            _logger.Information("Order {OrderId} completed", orderId);
        }
    }

    private async Task ValidateAsync(string orderId)
    {
        await Task.Delay(20); // simulated async I/O
        _logger.Information("Validated order {OrderId}", orderId);
    }

    private async Task ChargePaymentAsync(string orderId)
    {
        await Task.Delay(20); // simulated async I/O
        _logger.Information("Charged payment for order {OrderId}", orderId);
    }
}
