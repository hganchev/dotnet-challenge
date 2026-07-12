using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Relationships.Data;

/// <summary>
/// Lets `dotnet ef migrations add` / `dotnet ef database update` construct a
/// <see cref="LibraryDbContext"/> at design time.
/// </summary>
public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        optionsBuilder.UseSqlite("Data Source=library.db");
        return new LibraryDbContext(optionsBuilder.Options);
    }
}
