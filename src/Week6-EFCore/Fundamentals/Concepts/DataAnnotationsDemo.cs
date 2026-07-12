using Fundamentals.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Concepts;

/// <summary>
/// Highlights the constraints on <see cref="Fundamentals.Entities.Author"/>
/// that come purely from data annotations: <c>[Required]</c>, <c>[MaxLength]</c>
/// and <c>[Column]</c>/<c>[Table]</c>.
/// </summary>
public static class DataAnnotationsDemo
{
    public static void Run(AppDbContext context)
    {
        Console.WriteLine("--- Data Annotations ---");

        var authorType = context.Model.FindEntityType(typeof(Entities.Author))!;
        Console.WriteLine($"Author maps to table: {authorType.GetTableName()}");

        foreach (var property in authorType.GetProperties())
        {
            var maxLength = property.GetMaxLength();
            var isRequired = !property.IsNullable;
            var columnName = property.GetColumnName();
            Console.WriteLine($"  {property.Name} -> column '{columnName}', required={isRequired}, maxLength={maxLength?.ToString() ?? "unbounded"}");
        }

        Console.WriteLine();
    }
}
