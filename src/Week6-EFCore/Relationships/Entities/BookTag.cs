namespace Relationships.Entities;

/// <summary>
/// Explicit join entity for the Book &lt;-&gt; Tag many-to-many relationship.
/// An explicit join entity (rather than EF's implicit join table) is required
/// because the relationship itself carries data: <see cref="AddedAt"/>.
/// </summary>
public class BookTag
{
    public int BookId { get; set; }

    public Book Book { get; set; } = null!;

    public int TagId { get; set; }

    public Tag Tag { get; set; } = null!;

    public DateTime AddedAt { get; set; }
}
