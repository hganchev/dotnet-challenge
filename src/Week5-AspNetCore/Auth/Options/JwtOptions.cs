namespace Auth.Options;

/// <summary>
/// JWT signing/validation configuration bound from the "Jwt" section. In a real
/// application, <see cref="SigningKey"/> must come from an environment variable or a
/// secrets manager (Azure Key Vault, AWS Secrets Manager, etc.) — never commit it to
/// source control. Here it lives in appsettings.Development.json purely for local demo
/// purposes.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 15;
}
