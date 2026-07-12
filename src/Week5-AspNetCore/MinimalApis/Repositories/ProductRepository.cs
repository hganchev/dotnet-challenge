using System.Collections.Concurrent;
using MinimalApis.Models;

namespace MinimalApis.Repositories;

/// <summary>In-memory store for the CRUD demo. Registered as a Singleton.</summary>
public interface IProductRepository
{
    IReadOnlyCollection<Product> GetAll();
    Product? GetById(int id);
    Product Add(string name, decimal price);
    bool Update(int id, string name, decimal price);
    bool Delete(int id);
}

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId;

    public InMemoryProductRepository()
    {
        Add("Laptop", 1299.99m);
        Add("Wireless Mouse", 24.50m);
        Add("Mechanical Keyboard", 89.00m);
    }

    public IReadOnlyCollection<Product> GetAll() => _products.Values.OrderBy(p => p.Id).ToArray();

    public Product? GetById(int id) => _products.GetValueOrDefault(id);

    public Product Add(string name, decimal price)
    {
        var id = Interlocked.Increment(ref _nextId);
        var product = new Product { Id = id, Name = name, Price = price };
        _products[id] = product;
        return product;
    }

    public bool Update(int id, string name, decimal price)
    {
        if (!_products.TryGetValue(id, out var existing))
        {
            return false;
        }

        existing.Name = name;
        existing.Price = price;
        return true;
    }

    public bool Delete(int id) => _products.TryRemove(id, out _);
}
