namespace OOP.Encapsulation;

// Simple data class used to demonstrate object initializers and init-only setters.
public class PersonInfo
{
    // init-only: settable in constructors and object initializers, not reassignable afterwards
    public string FirstName { get; init; } = string.Empty;
    public string LastName  { get; init; } = string.Empty;

    // Regular auto-property: readable and writable after construction
    public int    Age   { get; set; }
    public string Email { get; set; } = string.Empty;

    // Computed / expression-bodied property
    public string FullName => $"{FirstName} {LastName}";

    public override string ToString() => $"{FullName}, age {Age} ({Email})";
}
