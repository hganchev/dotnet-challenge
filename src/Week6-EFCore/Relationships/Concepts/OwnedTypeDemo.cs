using Microsoft.EntityFrameworkCore;
using Relationships.Data;

namespace Relationships.Concepts;

/// <summary>
/// <c>OwnsOne()</c> models <c>Address</c> as a value object embedded in the
/// <c>AuthorProfiles</c> table — no separate <c>DbSet&lt;Address&gt;</c> or table exists.
/// </summary>
public static class OwnedTypeDemo
{
    public static async Task RunAsync(LibraryDbContext context)
    {
        Console.WriteLine("--- Owned Type (AuthorProfile.Address) ---");

        var profile = await context.AuthorProfiles.Include(p => p.Author).FirstAsync();
        Console.WriteLine($"{profile.Author.Name} lives at {profile.Address.Street}, {profile.Address.City} {profile.Address.PostalCode}");

        var addressEntityType = context.Model.FindEntityType(typeof(Entities.AuthorProfile))!
            .FindNavigation(nameof(Entities.AuthorProfile.Address))!
            .TargetEntityType;
        Console.WriteLine($"Address has no own table — it shares '{addressEntityType.GetTableName()}' with AuthorProfile.");

        Console.WriteLine();
    }
}
