using Fundamentals.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Concepts;

/// <summary>
/// Shows what EF Core infers by convention alone: primary keys named
/// <c>Id</c>/<c>&lt;Type&gt;Id</c>, required-ness from non-nullable reference
/// types, and foreign keys from navigation + shadow properties.
/// </summary>
public static class ConventionsDemo
{
    public static void Run(AppDbContext context)
    {
        Console.WriteLine("--- Conventions ---");

        var model = context.Model;
        foreach (var entityType in model.GetEntityTypes())
        {
            var primaryKey = entityType.FindPrimaryKey();
            Console.WriteLine($"Entity: {entityType.ClrType.Name}");
            Console.WriteLine($"  Table: {entityType.GetTableName()}");
            Console.WriteLine($"  Primary key (convention/annotation): {string.Join(", ", primaryKey?.Properties.Select(p => p.Name) ?? [])}");

            foreach (var fk in entityType.GetForeignKeys())
            {
                Console.WriteLine($"  Foreign key: {string.Join(", ", fk.Properties.Select(p => p.Name))} -> {fk.PrincipalEntityType.ClrType.Name}");
            }
        }

        Console.WriteLine();
    }
}
