using Generics.GenericClasses;

namespace Generics.Examples;

// Custom generic interfaces
public interface IRepository<T> where T : class
{
    void Add(T item);
    T? FindById(int id);
    IEnumerable<T> GetAll();
    bool Remove(T item);
}

// Entity base class providing an Id
public abstract class Entity
{
    public int Id { get; set; }
}

// Concrete entity
public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public override string ToString() => $"Customer #{Id}: {Name} <{Email}>";
}

// Concrete repository using the generic repository under the hood
public class CustomerRepository : IRepository<Customer>
{
    private readonly GenericRepository<Customer> _inner = new();

    public void Add(Customer item) => _inner.Add(item);

    public Customer? FindById(int id) => _inner.Find(c => c.Id == id);

    public IEnumerable<Customer> GetAll() => _inner.GetAll();

    public bool Remove(Customer item) => _inner.Remove(item);
}

public static class GenericInterfaces
{
    public static void Run()
    {
        Console.WriteLine("\n=== Generic Interfaces: IRepository<T>, IEnumerable<T> ===");

        IRepository<Customer> repo = new CustomerRepository();
        repo.Add(new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" });
        repo.Add(new Customer { Id = 2, Name = "Bob",   Email = "bob@example.com" });
        repo.Add(new Customer { Id = 3, Name = "Carol", Email = "carol@example.com" });

        Console.WriteLine("All customers:");
        // IEnumerable<T> — foreach works because GetAll() returns IEnumerable<Customer>
        foreach (var c in repo.GetAll())
            Console.WriteLine($"  {c}");

        var found = repo.FindById(2);
        Console.WriteLine($"FindById(2): {found}");

        var notFound = repo.FindById(99);
        Console.WriteLine($"FindById(99): {notFound?.ToString() ?? "(not found)"}");
    }
}
