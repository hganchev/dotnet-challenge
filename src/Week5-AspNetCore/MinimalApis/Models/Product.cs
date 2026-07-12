namespace MinimalApis.Models;

/// <summary>Domain entity used by the CRUD demo endpoints.</summary>
public sealed class Product
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
}
