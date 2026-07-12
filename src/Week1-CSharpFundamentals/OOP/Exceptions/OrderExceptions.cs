namespace OOP.Exceptions;

/// <summary>
/// Custom exception carrying domain context (<see cref="OrderId"/>) via a
/// constructor parameter/property — not by parsing the message string.
/// Derives from <see cref="Exception"/> directly (never <c>ApplicationException</c>,
/// which offers no benefit over the base type).
/// </summary>
public sealed class OrderNotFoundException : Exception
{
    public int OrderId { get; }

    public OrderNotFoundException(int orderId)
        : base($"Order {orderId} was not found.")
    {
        OrderId = orderId;
    }

    public OrderNotFoundException(int orderId, Exception innerException)
        : base($"Order {orderId} was not found.", innerException)
    {
        OrderId = orderId;
    }
}

/// <summary>
/// A second, distinct domain exception used to show that top-level handlers
/// should catch specific types rather than a bare <see cref="Exception"/>.
/// </summary>
public sealed class InsufficientStockException : Exception
{
    public string Sku { get; }
    public int Requested { get; }
    public int Available { get; }

    public InsufficientStockException(string sku, int requested, int available)
        : base($"Cannot reserve {requested} of '{sku}' — only {available} available.")
    {
        Sku = sku;
        Requested = requested;
        Available = available;
    }
}
