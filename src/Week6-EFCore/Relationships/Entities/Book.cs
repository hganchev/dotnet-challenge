namespace Relationships.Entities;

/// <summary>
/// One-to-many dependent (<see cref="Author"/> -> Books) and many-to-many
/// participant (Books &lt;-&gt; <see cref="Tag"/> via the explicit <see cref="BookTag"/> join entity).
/// </summary>
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
}
