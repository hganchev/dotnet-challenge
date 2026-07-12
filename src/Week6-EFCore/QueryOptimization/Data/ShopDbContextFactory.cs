using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QueryOptimization.Data;

/// <summary>
/// Lets `dotnet ef migrations add` / `dotnet ef database update` construct a
/// <see cref="ShopDbContext"/> at design time.
/// </summary>
public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();
        optionsBuilder.UseSqlite("Data Source=shop.db");
        return new ShopDbContext(optionsBuilder.Options);
    }
}
