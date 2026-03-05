namespace ValueVsReference.Records;

// Record inheritance: positional params must be forwarded to the base constructor.
public record Animal(string Name);
public record Dog(string Name, string Breed) : Animal(Name);
