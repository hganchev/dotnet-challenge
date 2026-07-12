namespace DependencyInjection.Options;

/// <summary>
/// Strongly-typed configuration bound from the "MyFeature" section of appsettings.json.
/// Consumed via <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/> /
/// <see cref="Microsoft.Extensions.Options.IOptionsSnapshot{TOptions}"/> instead of
/// injecting raw <c>IConfiguration</c> into business logic.
/// </summary>
public sealed class MyFeatureOptions
{
    public const string SectionName = "MyFeature";

    public string Greeting { get; init; } = string.Empty;
    public int MaxItems { get; init; }
}
