using Microsoft.EntityFrameworkCore;
using Relationships.Data;

namespace Relationships.Concepts;

/// <summary>
/// Many-to-many via an explicit join entity (<c>BookTag</c>) rather than EF's
/// implicit join table, because the relationship carries a payload (<c>AddedAt</c>).
/// </summary>
public static class ManyToManyDemo
{
    public static async Task RunAsync(LibraryDbContext context)
    {
        Console.WriteLine("--- Many-to-Many (Book <-> Tag via BookTag) ---");

        var books = await context.Books
            .Include(b => b.BookTags)
            .ThenInclude(bt => bt.Tag)
            .ToListAsync();

        foreach (var book in books)
        {
            var tags = book.BookTags.Select(bt => $"{bt.Tag.Name} (added {bt.AddedAt:yyyy-MM-dd})");
            Console.WriteLine($"{book.Title}: {string.Join(", ", tags)}");
        }

        Console.WriteLine();
    }
}
