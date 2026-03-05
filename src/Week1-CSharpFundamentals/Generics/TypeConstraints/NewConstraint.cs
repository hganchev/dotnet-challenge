namespace Generics.TypeConstraints;

// where T : new() — T must have a public parameterless constructor
// Enables: new T() inside generic code — used in factory / activator patterns
public class Factory<T> where T : new()
{
    public T Create() => new T();

    public T[] CreateMany(int count)
    {
        var items = new T[count];
        for (int i = 0; i < count; i++)
            items[i] = new T();
        return items;
    }
}

// Example class usable with new() constraint
public class Widget
{
    public Guid Id { get; } = Guid.NewGuid();
    public override string ToString() => $"Widget {Id.ToString()[..8]}";
}

public static class NewConstraintExamples
{
    // Generic initializer — creates and optionally initializes an instance
    public static T CreateAndInit<T>(Action<T>? init = null) where T : new()
    {
        var instance = new T();
        init?.Invoke(instance);
        return instance;
    }

    public static void Run()
    {
        Console.WriteLine("\n=== where T : new() ===");

        var factory = new Factory<Widget>();
        var single = factory.Create();
        Console.WriteLine($"Created: {single}");

        var batch = factory.CreateMany(3);
        Console.WriteLine($"Batch of {batch.Length}:");
        foreach (var w in batch)
            Console.WriteLine($"  {w}");
    }
}
