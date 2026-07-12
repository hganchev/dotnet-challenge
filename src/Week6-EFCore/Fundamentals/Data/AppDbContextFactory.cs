using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fundamentals.Data;

/// <summary>
/// Lets `dotnet ef migrations add` / `dotnet ef database update` construct an
/// <see cref="AppDbContext"/> at design time without running the full
/// application host.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=fundamentals.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}
