using Fundamentals.Data;
using Fundamentals.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Concepts;

/// <summary>
/// Demonstrates three EF Core fundamentals not covered elsewhere in this project:
/// seeded lookup data (<c>HasData</c>), an explicit transaction spanning more
/// than one <c>SaveChangesAsync()</c> call, and optimistic-concurrency conflict
/// detection via a manually incremented <c>Version</c> token.
/// </summary>
public static class ConcurrencyAndTransactionsDemo
{
    public static async Task RunAsync(AppDbContext context)
    {
        Console.WriteLine("--- Seeded Lookup Data (HasData) ---");
        var genres = await context.Genres.OrderBy(g => g.Id).ToListAsync();
        foreach (var genre in genres)
        {
            Console.WriteLine($"  Genre {genre.Id}: {genre.Name}");
        }

        Console.WriteLine("\n--- Explicit Transaction Across Two SaveChangesAsync() Calls ---");
        var author = await context.Authors.FirstAsync();

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var book = new Book
            {
                Title = "Sci-Fi Sampler",
                Price = 9.99m,
                AuthorId = author.Id,
                GenreId = genres.Single(g => g.Name == "Science Fiction").Id
            };
            context.Books.Add(book);
            await context.SaveChangesAsync(); // first unit of work

            book.Price = 8.49m; // simulate a follow-up business rule applied after insert
            await context.SaveChangesAsync(); // second unit of work, same transaction

            await transaction.CommitAsync();
            Console.WriteLine($"  Committed: '{book.Title}' inserted then re-priced to {book.Price:C} in one transaction.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        Console.WriteLine("\n--- Optimistic Concurrency Conflict (Version token) ---");
        var bookId = (await context.Books.FirstAsync()).Id;

        // Two independent contexts (same connection string) simulate two users
        // loading the same row concurrently.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(context.Database.GetConnectionString())
            .Options;
        await using var contextA = new AppDbContext(options);
        await using var contextB = new AppDbContext(options);

        var bookA = await contextA.Books.FirstAsync(b => b.Id == bookId);
        var bookB = await contextB.Books.FirstAsync(b => b.Id == bookId);

        bookA.Price += 1.00m;
        bookA.Version++; // SQLite has no database-generated rowversion — the app owns the bump
        await contextA.SaveChangesAsync();
        Console.WriteLine($"  User A saved first: price is now {bookA.Price:C}, version {bookA.Version}.");

        bookB.Price += 5.00m; // bookB still holds the Version read before A's update
        bookB.Version++;
        try
        {
            await contextB.SaveChangesAsync();
            Console.WriteLine("  User B saved without conflict (unexpected).");
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("  User B's save threw DbUpdateConcurrencyException — Version no longer matched the row A already updated.");
        }

        Console.WriteLine();
    }
}
