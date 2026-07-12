namespace Relationships.Entities;

/// <summary>Many-to-many participant on the tag side.</summary>
public class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
}
