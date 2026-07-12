using Microsoft.EntityFrameworkCore;
using SampleApi.Models;

namespace SampleApi.Data;

/// <summary>EF Core context for the product catalog. Backed by an in-memory provider so the
/// sample API and its integration tests never require a real database.</summary>
public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
