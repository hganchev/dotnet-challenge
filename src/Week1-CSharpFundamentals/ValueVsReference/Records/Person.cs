namespace ValueVsReference.Records;

// Nominal record class: explicit init-only properties, custom ToString.
public record Person
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName  { get; init; } = string.Empty;
    public int    Age       { get; init; }

    public override string ToString() => $"{FirstName} {LastName}, age {Age}";
}
