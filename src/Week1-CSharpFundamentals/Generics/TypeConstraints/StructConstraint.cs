namespace Generics.TypeConstraints;

// where T : struct — T must be a non-nullable value type
// Enables: default(T) always has a known zero-value, T? (Nullable<T>) is valid
public class ValueHolder<T> where T : struct
{
    private T _value;

    public ValueHolder() => _value = default; // default(T) is safe: 0, false, Guid.Empty, etc.

    public ValueHolder(T value) => _value = value;

    public T Value => _value;

    // Wrapping in Nullable<T> is only valid because T : struct
    public T? ToNullable() => _value;

    public bool IsDefault() => EqualityComparer<T>.Default.Equals(_value, default);
}

public static class StructConstraintExamples
{
    // default(T) — zero-value for any value type
    public static T ZeroValue<T>() where T : struct => default;

    public static void Run()
    {
        Console.WriteLine("\n=== where T : struct + default(T) ===");

        Console.WriteLine($"default(int)    = {ZeroValue<int>()}");
        Console.WriteLine($"default(bool)   = {ZeroValue<bool>()}");
        Console.WriteLine($"default(double) = {ZeroValue<double>()}");
        Console.WriteLine($"default(Guid)   = {ZeroValue<Guid>()}");

        var holder = new ValueHolder<int>(42);
        Console.WriteLine($"Holder value: {holder.Value}, IsDefault: {holder.IsDefault()}");

        var emptyHolder = new ValueHolder<int>();
        Console.WriteLine($"Empty holder IsDefault: {emptyHolder.IsDefault()}");

        int? nullable = holder.ToNullable();
        Console.WriteLine($"As Nullable<int>: {nullable}");
    }
}
