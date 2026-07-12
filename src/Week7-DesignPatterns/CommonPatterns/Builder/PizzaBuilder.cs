namespace CommonPatterns.Builder;

/// <summary>Complex immutable object assembled step by step.</summary>
public sealed class Pizza
{
    public required string Size { get; init; }
    public IReadOnlyList<string> Toppings { get; init; } = [];
    public bool ExtraCheese { get; init; }

    public override string ToString() =>
        $"{Size} pizza with [{string.Join(", ", Toppings)}]{(ExtraCheese ? " + extra cheese" : string.Empty)}";
}

/// <summary>Fluent Builder — each method returns <c>this</c> so calls chain; <see cref="Build"/>
/// produces the final immutable object.</summary>
public sealed class PizzaBuilder
{
    private string _size = "Medium";
    private readonly List<string> _toppings = [];
    private bool _extraCheese;

    public PizzaBuilder WithSize(string size)
    {
        _size = size;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        _toppings.Add(topping);
        return this;
    }

    public PizzaBuilder WithExtraCheese()
    {
        _extraCheese = true;
        return this;
    }

    public Pizza Build() => new()
    {
        Size = _size,
        Toppings = _toppings.AsReadOnly(),
        ExtraCheese = _extraCheese
    };
}
