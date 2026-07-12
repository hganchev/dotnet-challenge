using Microsoft.EntityFrameworkCore;
using Relationships.Data;

namespace Relationships.Concepts;

/// <summary>One-to-many: <c>Author -> many Books</c>, the most common relationship shape.</summary>
public static class OneToManyDemo
{
    public static async Task RunAsync(LibraryDbContext context)
    {
        Console.WriteLine("--- One-to-Many (Author -> Books) ---");

        var authors = await context.Authors
            .Include(a => a.Books)
            .ToListAsync();

        foreach (var author in authors)
        {
            Console.WriteLine($"{author.Name} wrote {author.Books.Count} book(s): {string.Join(", ", author.Books.Select(b => b.Title))}");
        }

        Console.WriteLine();
    }
}
