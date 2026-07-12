using Fundamentals.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Concepts;

/// <summary>
/// Highlights the constraints on <see cref="Fundamentals.Entities.Book"/> that
/// are configured entirely through <c>OnModelCreating</c> Fluent API rather
/// than data annotations.
/// </summary>
public static class FluentApiDemo
{
    public static void Run(AppDbContext context)
    {
        Console.WriteLine("--- Fluent API ---");

        var bookType = context.Model.FindEntityType(typeof(Entities.Book))!;
        Console.WriteLine($"Book maps to table: {bookType.GetTableName()}");

        var titleProperty = bookType.FindProperty(nameof(Entities.Book.Title))!;
        Console.WriteLine($"  Title -> required={!titleProperty.IsNullable}, maxLength={titleProperty.GetMaxLength()}");

        var priceProperty = bookType.FindProperty(nameof(Entities.Book.Price))!;
        Console.WriteLine($"  Price -> column type={priceProperty.GetColumnType()}");

        var fk = bookType.GetForeignKeys().Single(f => f.PrincipalEntityType.ClrType == typeof(Entities.Author));
        Console.WriteLine($"  Author relationship -> DeleteBehavior={fk.DeleteBehavior}");

        Console.WriteLine();
    }
}
