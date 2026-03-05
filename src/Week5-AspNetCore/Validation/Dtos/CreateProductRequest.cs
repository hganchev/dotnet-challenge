namespace Validation.Dtos;

/// <summary>
/// Represents a product creation request in the <b>FluentValidation</b> approach.
/// The DTO is intentionally free of validation concerns — rules live in a dedicated
/// <c>AbstractValidator&lt;T&gt;</c> class, keeping the DTO as a pure data carrier.
/// Compare with <see cref="CreateProductRequestAnnotated"/> used in the built-in path.
/// </summary>
public sealed record CreateProductRequest(string? Name, decimal Price, string? Category);
