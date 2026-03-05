namespace ValueVsReference.EqualitySemantics;

// Struct with IEquatable<T>: type-safe equality without boxing; == / != overloaded.
public struct Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }

    public bool Equals(Money other) =>
        Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) =>
        obj is Money m && Equals(m);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public static bool operator ==(Money a, Money b) => a.Equals(b);
    public static bool operator !=(Money a, Money b) => !a.Equals(b);

    public override string ToString() => $"{Amount} {Currency}";
}
