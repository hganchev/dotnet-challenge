using Microsoft.EntityFrameworkCore;
using Relationships.Data;
using Relationships.Entities;

namespace Relationships.Concepts;

/// <summary>One-to-one: <see cref="Author"/> &lt;-&gt; <see cref="AuthorProfile"/>.</summary>
public static class OneToOneDemo
{
    public static async Task RunAsync(LibraryDbContext context)
    {
        Console.WriteLine("--- One-to-One (Author <-> AuthorProfile) ---");

        var author = await context.Authors
            .Include(a => a.Profile)
            .FirstAsync(a => a.Name == "Ada Lovelace");

        Console.WriteLine($"{author.Name}'s profile bio: {author.Profile!.Bio}");
        Console.WriteLine($"Profile PK doubles as FK: AuthorProfile.AuthorId == Author.Id -> {author.Profile.AuthorId == author.Id}");

        Console.WriteLine();
    }
}
