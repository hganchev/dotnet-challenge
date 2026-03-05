using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Validation.Dtos;

namespace Validation.Filters;

/// <summary>
/// A generic Minimal API <see cref="IEndpointFilter"/> that validates the first
/// argument of type <typeparamref name="T"/> using a registered
/// <see cref="IValidator{T}"/> (FluentValidation).
/// Short-circuits the pipeline with a 400 <c>ValidationProblem</c> on failure.
///
/// .NET 10 built-in equivalent: calling <c>builder.Services.AddValidation()</c>
/// replaces this filter entirely — see <c>ValidationFilter.BuiltIn.cs</c> for details.
/// </summary>
public sealed class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
            return await next(context);

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return await next(context);

        var result = await validator.ValidateAsync(argument);
        if (!result.IsValid)
        {
            // Group errors by property name — same shape as the built-in 400 response
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}
