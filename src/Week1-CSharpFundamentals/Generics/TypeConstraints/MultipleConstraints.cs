namespace Generics.TypeConstraints;

// Combining multiple where clauses on the same or different type parameters

// T must be a class, implement IComparable<T>, and have a parameterless constructor
public class ComparableFactory<T> where T : class, IComparable<T>, new()
{
    public T CreateDefault() => new T();

    public T Larger(T a, T b) => a.CompareTo(b) >= 0 ? a : b;
}

// Multiple type parameters each with their own constraints
public class Transformer<TSource, TResult>
    where TSource : class
    where TResult : class, new()
{
    private readonly Func<TSource, TResult> _transform;

    public Transformer(Func<TSource, TResult> transform) => _transform = transform;

    public TResult Transform(TSource source) => _transform(source);

    public IEnumerable<TResult> TransformAll(IEnumerable<TSource> sources)
        => sources.Select(_transform);
}

// Concrete class satisfying: class + IComparable<T> + new()
public class NamedItem : IComparable<NamedItem>
{
    public string Name { get; set; } = string.Empty;

    public int CompareTo(NamedItem? other)
        => string.Compare(Name, other?.Name, StringComparison.Ordinal);

    public override string ToString() => Name;
}

public static class MultipleConstraintsExamples
{
    public static void Run()
    {
        Console.WriteLine("\n=== Multiple where constraints ===");

        var factory = new ComparableFactory<NamedItem>();
        var a = new NamedItem { Name = "Alpha" };
        var b = new NamedItem { Name = "Beta" };
        Console.WriteLine($"Larger of '{a}' and '{b}': {factory.Larger(a, b)}");

        var transformer = new Transformer<NamedItem, NamedItem>(
            src => new NamedItem { Name = src.Name.ToUpper() }
        );
        var items = new[] { new NamedItem { Name = "foo" }, new NamedItem { Name = "bar" } };
        var results = transformer.TransformAll(items);
        Console.WriteLine($"Transformed: [{string.Join(", ", results)}]");
    }
}
