using FluentValidation;
using Validation.Dtos;

namespace Validation.Validators;

/// <summary>
/// FluentValidation validator for <see cref="CreateProductRequest"/>.
/// Rules are defined in code — the DTO stays attribute-free.
/// Registered via <c>AddValidatorsFromAssemblyContaining&lt;Program&gt;()</c> in Program.cs.
/// The matching .NET 10 built-in approach lives in <c>CreateProductRequestAnnotated.cs</c>.
/// </summary>
public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    private static readonly string[] _allowedCategories = ["Electronics", "Clothing", "Food"];

    public CreateProductRequestValidator()
    {
        // Built-in equivalent: [Required] + [MaxLength(100)] on the Name parameter
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        // Built-in equivalent: [Range(0.01, double.MaxValue)] on the Price parameter
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        // Built-in equivalent: [Required] + [AllowedValues(...)] on the Category parameter
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .Must(c => c is not null && _allowedCategories.Contains(c))
            .WithMessage("Category must be Electronics, Clothing, or Food.");

        // Cross-field rule — built-in equivalent: IValidatableObject.Validate()
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(10.00m)
            .When(x => x.Category == "Electronics")
            .WithMessage("Electronics must be priced at least 10.00.");
    }
}
