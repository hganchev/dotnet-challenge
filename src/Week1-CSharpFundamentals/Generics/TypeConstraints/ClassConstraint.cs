namespace Generics.TypeConstraints;

// where T : class — T must be a reference type
// Enables: null checks, reference equality, nullable T?
public class NullableWrapper<T> where T : class
{
    private T? _value;

    public bool HasValue => _value is not null;

    public T Value => _value ?? throw new InvalidOperationException("No value set.");

    public void Set(T value) => _value = value;

    public void Clear() => _value = null;

    // Only possible because T : class — value types cannot be null
    public T? GetOrDefault() => _value;
}

public static class ClassConstraintExamples
{
    public static void Run()
    {
        Console.WriteLine("=== where T : class ===");

        var wrapper = new NullableWrapper<string>();
        Console.WriteLine($"HasValue before set: {wrapper.HasValue}");

        wrapper.Set("hello");
        Console.WriteLine($"HasValue after set: {wrapper.HasValue}, Value: {wrapper.Value}");

        wrapper.Clear();
        Console.WriteLine($"HasValue after clear: {wrapper.HasValue}");
        Console.WriteLine($"GetOrDefault: {wrapper.GetOrDefault() ?? "(null)"}");
    }
}
