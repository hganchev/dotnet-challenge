namespace CommonPatterns.Repository;

public sealed class Product
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
}

/// <summary>Repository abstraction — a collection-like interface over storage. Deliberately
/// does NOT expose <c>IQueryable&lt;T&gt;</c> so the domain layer stays store-agnostic and the
/// implementation (SQL, in-memory, document DB, …) can change without leaking query semantics.</summary>
public interface IRepository<T> where T : class
{
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    void Add(T item);
    void Remove(Guid id);
}

/// <summary>Concrete adapter — swappable for an EF Core/Dapper-backed implementation without
/// changing any consumer, because consumers depend only on <see cref="IRepository{T}"/> (DIP).</summary>
public sealed class InMemoryProductRepository : IRepository<Product>
{
    private readonly Dictionary<Guid, Product> _store = [];

    public Product? GetById(Guid id) => _store.GetValueOrDefault(id);

    public IReadOnlyList<Product> GetAll() => [.. _store.Values];

    public void Add(Product item) => _store[item.Id] = item;

    public void Remove(Guid id) => _store.Remove(id);
}
