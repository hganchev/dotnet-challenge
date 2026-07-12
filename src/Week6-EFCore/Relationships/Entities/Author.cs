namespace Relationships.Entities;

/// <summary>
/// One-to-many principal: an <see cref="Author"/> writes many <see cref="Book"/>s,
/// and optionally has a single <see cref="AuthorProfile"/> (one-to-one).
/// </summary>
public class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();

    /// <summary>Reference navigation for the one-to-one relationship.</summary>
    public AuthorProfile? Profile { get; set; }
}
