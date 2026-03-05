namespace Generics.GenericClasses;

// Generic repository pattern - works with any entity type
// Demonstrates the "write a generic repository" expected outcome from week1 notes
public class GenericRepository<T> where T : class
{
    private readonly List<T> _store = [];

    public void Add(T entity) => _store.Add(entity);

    public bool Remove(T entity) => _store.Remove(entity);

    public IReadOnlyList<T> GetAll() => _store.AsReadOnly();

    public T? Find(Func<T, bool> predicate) => _store.FirstOrDefault(predicate);

    public IEnumerable<T> Filter(Func<T, bool> predicate) => _store.Where(predicate);

    public int Count => _store.Count;
}

// Example entity used in demos
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public override string ToString() => $"Product #{Id}: {Name} (${Price:F2})";
}
