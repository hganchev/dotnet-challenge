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
}
