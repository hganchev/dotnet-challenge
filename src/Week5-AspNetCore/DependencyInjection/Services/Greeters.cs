namespace DependencyInjection.Services;

/// <summary>
/// Two implementations of the same interface, registered as <b>keyed services</b>
/// (.NET 8+) so callers can request a specific one via <c>[FromKeyedServices]</c>
/// instead of needing separate interfaces per language.
/// </summary>
public interface IGreeter
{
    string Greet(string name);
}

public sealed class EnglishGreeter : IGreeter
{
    public string Greet(string name) => $"Hello, {name}!";
}

public sealed class BulgarianGreeter : IGreeter
{
    public string Greet(string name) => $"Здравей, {name}!";
}
