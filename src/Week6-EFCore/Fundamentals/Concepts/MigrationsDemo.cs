using Fundamentals.Data;
using Fundamentals.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Concepts;

/// <summary>
/// Applies pending migrations, then inspects the <c>__EFMigrationsHistory</c>
/// table that EF Core uses to track which migrations have already run.
/// </summary>
public static class MigrationsDemo
{
    public static async Task RunAsync(AppDbContext context)
    {
        Console.WriteLine("--- Migrations ---");

        var pending = (await context.Database.GetPendingMigrationsAsync()).ToList();
        Console.WriteLine(pending.Count > 0
            ? $"Applying pending migrations: {string.Join(", ", pending)}"
            : "No pending migrations.");

        await context.Database.MigrateAsync();

        var applied = await context.Database.GetAppliedMigrationsAsync();
        Console.WriteLine("Applied migrations (__EFMigrationsHistory):");
        foreach (var migration in applied)
        {
            Console.WriteLine($"  {migration}");
        }

        if (!context.Authors.Any())
        {
            context.Authors.Add(new Author
            {
                Name = "Ada Lovelace",
                Email = "ada@example.com",
                Books =
                {
                    new Book { Title = "Notes on the Analytical Engine", Price = 12.50m }
                }
            });
            await context.SaveChangesAsync();
            Console.WriteLine("Seeded one author with one book.");
        }

        Console.WriteLine();
    }
}
