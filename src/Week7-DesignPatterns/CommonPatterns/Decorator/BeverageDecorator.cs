namespace CommonPatterns.Decorator;

/// <summary>Component abstraction — decorators wrap this interface, never a concrete class.</summary>
public interface IBeverage
{
    string Description { get; }
    decimal Cost { get; }
}

public sealed class Espresso : IBeverage
{
    public string Description => "Espresso";
    public decimal Cost => 2.00m;
}

public sealed class HouseBlend : IBeverage
{
    public string Description => "House Blend Coffee";
    public decimal Cost => 1.50m;
}

/// <summary>Base decorator — wraps an <see cref="IBeverage"/> and adds behaviour without
/// inheriting from any concrete beverage.</summary>
public abstract class BeverageDecorator(IBeverage beverage) : IBeverage
{
    protected IBeverage Inner { get; } = beverage;

    public abstract string Description { get; }
    public abstract decimal Cost { get; }
}

public sealed class MilkDecorator(IBeverage beverage) : BeverageDecorator(beverage)
{
    public override string Description => $"{Inner.Description} + Milk";
    public override decimal Cost => Inner.Cost + 0.50m;
}

public sealed class SugarDecorator(IBeverage beverage) : BeverageDecorator(beverage)
{
    public override string Description => $"{Inner.Description} + Sugar";
    public override decimal Cost => Inner.Cost + 0.25m;
}

public sealed class WhipDecorator(IBeverage beverage) : BeverageDecorator(beverage)
{
    public override string Description => $"{Inner.Description} + Whip";
    public override decimal Cost => Inner.Cost + 0.75m;
}
