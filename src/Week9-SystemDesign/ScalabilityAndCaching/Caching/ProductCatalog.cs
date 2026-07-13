namespace ScalabilityAndCaching.Caching;

/// <summary>Simulates a slow backing store (e.g. a database) so the caching demos can show
/// how many times the "expensive" call actually happens.</summary>
public class ProductCatalog
{
    private readonly Dictionary<int, Product> _products = new()
    {
        [1] = new Product(1, "Mechanical Keyboard", 79.99m),
        [2] = new Product(2, "Wireless Mouse", 24.99m),
    };

    private int _dbCallCount;

    public int DbCallCount => _dbCallCount;

    public async Task<Product?> GetProductFromDatabaseAsync(int id)
    {
        Interlocked.Increment(ref _dbCallCount);
        await Task.Delay(200); // simulate slow I/O
        return _products.GetValueOrDefault(id);
    }
}
