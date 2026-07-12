namespace CommonPatterns.Observer;

public sealed class PriceChangedEventArgs(decimal oldPrice, decimal newPrice) : EventArgs
{
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
}

/// <summary>Publisher — raises <see cref="PriceChanged"/> using the standard .NET event pattern.</summary>
public sealed class StockTicker(string symbol)
{
    public string Symbol { get; } = symbol;

    private decimal _price;

    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public decimal Price
    {
        get => _price;
        set
        {
            if (value == _price) return;
            var old = _price;
            _price = value;
            PriceChanged?.Invoke(this, new PriceChangedEventArgs(old, value));
        }
    }
}

/// <summary>Subscriber — must unsubscribe (<c>-=</c>) when done, otherwise the publisher keeps a
/// reference to it and it can never be garbage collected while the publisher is alive (memory leak).</summary>
public sealed class PriceAlertSubscriber(string name)
{
    public string Name { get; } = name;

    public void OnPriceChanged(object? sender, PriceChangedEventArgs e)
    {
        var ticker = (StockTicker)sender!;
        var direction = e.NewPrice > e.OldPrice ? "up" : "down";
        Console.WriteLine($"  [{Name}] {ticker.Symbol} moved {direction}: {e.OldPrice:C} → {e.NewPrice:C}");
    }
}
