namespace Relationships.Entities;

/// <summary>
/// Owned type (value object) — has no identity of its own and is always
/// configured via <c>OwnsOne()</c> on its owner, mapping to columns on the
/// owner's table rather than a separate one.
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;
}
