using Microsoft.EntityFrameworkCore;
using Relationships.Data;
using Relationships.Entities;

namespace Relationships.Concepts;

/// <summary>
/// Demonstrates <see cref="DeleteBehavior"/> choices: Cascade removes an
/// Author's Books and Profile automatically; Restrict on Book -&gt; Tag
/// protects Tags from being deleted just because a Book referencing them is removed.
/// </summary>
public static class CascadeDeleteDemo
{
    public static async Task RunAsync(LibraryDbContext context)
    {
        Console.WriteLine("--- Cascade Delete Behaviors ---");

        var author = new Author
        {
            Name = "Temporary Author",
            Profile = new AuthorProfile { Bio = "Will be deleted with the author." },
            Books = { new Book { Title = "Temporary Book" } }
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var authorId = author.Id;
        var bookId = author.Books.First().Id;

        context.Authors.Remove(author);
        await context.SaveChangesAsync();

        var profileGone = await context.AuthorProfiles.FindAsync(authorId) is null;
        var bookGone = await context.Books.FindAsync(bookId) is null;
        Console.WriteLine($"Cascade delete removed AuthorProfile: {profileGone}, Book: {bookGone}");

        var tag = await context.Tags.FirstAsync();
        Console.WriteLine($"Tag '{tag.Name}' survives independently of any Book that references it (DeleteBehavior.Restrict on Book->Tag).");

        Console.WriteLine();
    }
}
