using System.ComponentModel.DataAnnotations;

namespace Validation.Dtos;

/// <summary>
/// Represents a product creation request in the <b>.NET 10 built-in validation</b> approach.
/// Validation rules are expressed directly as DataAnnotations attributes on the DTO.
/// Cross-field rules are handled via <see cref="IValidatableObject"/>.
/// Compare with <see cref="CreateProductRequest"/> + its separate validator used in the FV path.
/// </summary>
public sealed record CreateProductRequestAnnotated(
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    string? Name,

    // FluentValidation equivalent: RuleFor(x => x.Price).GreaterThan(0)
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    decimal Price,

    [Required(ErrorMessage = "Category is required.")]
    // [AllowedValues] was introduced in .NET 8 (System.ComponentModel.DataAnnotations)
    [AllowedValues("Electronics", "Clothing", "Food",
        ErrorMessage = "Category must be Electronics, Clothing, or Food.")]
    string? Category
) : IValidatableObject
{
    /// <summary>
    /// Cross-field validation — the built-in equivalent of FluentValidation's
    /// <c>.When()</c> / <c>.Must()</c> chained across properties.
    /// The framework calls this after all attribute-level rules pass.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // FluentValidation equivalent:
        //   RuleFor(x => x.Price)
        //       .GreaterThanOrEqualTo(10.00m)
        //       .When(x => x.Category == "Electronics")
        //       .WithMessage("Electronics must be priced at least 10.00.");
        if (Category == "Electronics" && Price < 10.00m)
        {
            yield return new ValidationResult(
                "Electronics must be priced at least 10.00.",
                [nameof(Price), nameof(Category)]);
        }
    }
}
