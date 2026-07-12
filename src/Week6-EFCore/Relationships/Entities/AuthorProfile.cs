namespace Relationships.Entities;

/// <summary>
/// Dependent side of the one-to-one relationship with <see cref="Author"/>.
/// Its primary key is also its foreign key (<see cref="AuthorId"/>), which is
/// how EF Core enforces a true 1:1 cardinality instead of 1:many.
/// </summary>
public class AuthorProfile
{
    public int AuthorId { get; set; }

    public string Bio { get; set; } = string.Empty;

    /// <summary>Owned value object — stored as columns on this table, no separate DbSet.</summary>
    public Address Address { get; set; } = new();

    public Author Author { get; set; } = null!;
}
