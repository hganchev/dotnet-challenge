using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundamentals.Entities;

/// <summary>
/// Configured primarily with data annotations to demonstrate attribute-based
/// EF Core configuration. <see cref="Id"/> and <see cref="Books"/> are picked
/// up entirely by convention (primary key + collection navigation).
/// </summary>
[Table("Authors")]
public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("EmailAddress")]
    [MaxLength(200)]
    public string? Email { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
