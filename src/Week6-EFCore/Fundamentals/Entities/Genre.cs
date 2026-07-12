namespace Fundamentals.Entities;

/// <summary>
/// Static reference/lookup data. Rows are seeded at migration-build time via
/// <c>HasData</c> in <see cref="Data.AppDbContext.OnModelCreating"/> — never
/// inserted with ad-hoc runtime code, so the same fixed rows exist in every
/// environment the migration is applied to.
/// </summary>
public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
