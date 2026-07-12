namespace Fundamentals.Entities;

/// <summary>
/// Deliberately free of data annotations — every constraint on this entity is
/// configured via Fluent API in <see cref="Data.AppDbContext.OnModelCreating"/>
/// to demonstrate the annotation-free configuration style.
/// </summary>
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    /// <summary>Optional reference to a seeded <see cref="Genre"/> lookup row.</summary>
    public int? GenreId { get; set; }

    public Genre? Genre { get; set; }

    /// <summary>
    /// Optimistic-concurrency token configured via <c>IsConcurrencyToken()</c> in
    /// <see cref="Data.AppDbContext.OnModelCreating"/>. SQLite has no
    /// database-generated rowversion column, so the application increments this
    /// value itself on every update; EF Core still includes the original value
    /// in the WHERE clause of generated UPDATE statements and throws
    /// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>
    /// when it no longer matches the row in the database.
    /// </summary>
    public uint Version { get; set; } = 1;
}
