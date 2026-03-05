namespace Generics.Variance;

// Covariance: out T — T can only appear in output positions (return types)
// Allows IReadOnlyContainer<Derived> to be assigned to IReadOnlyContainer<Base>
public interface IReadOnlyContainer<out T>
{
    T Get();
    int Count { get; }
}

// Concrete implementation
public class ReadOnlyContainer<T> : IReadOnlyContainer<T>
{
    private readonly T[] _items;

    public ReadOnlyContainer(params T[] items) => _items = items;

    public T Get() => _items[0];
    public int Count => _items.Length;
}

public static class CovariantExamples
{
    public static void Run()
    {
        Console.WriteLine("\n=== Covariance: out T (IReadOnlyContainer<out T>) ===");

        // IEnumerable<string> is assignable to IEnumerable<object> — covariance built in
        IEnumerable<string> strings = ["hello", "world"];
        IEnumerable<object> objects = strings; // valid — covariant
        Console.WriteLine($"IEnumerable<string> → IEnumerable<object>: [{string.Join(", ", objects)}]");

        // Custom covariant interface
        IReadOnlyContainer<string> strContainer = new ReadOnlyContainer<string>("alpha", "beta");
        IReadOnlyContainer<object> objContainer = strContainer; // valid — out T is covariant
        Console.WriteLine($"IReadOnlyContainer<string> → IReadOnlyContainer<object>: {objContainer.Get()}");

        // Why covariance is safe: we only READ from the container, never write
        // List<string> → List<object> would be INVALID (List<T> is invariant)
        // because you could then do list.Add(42) breaking type safety
        Console.WriteLine("List<string> → List<object>: INVALID (invariant) — compiler rejects it");
    }
}
