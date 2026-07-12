namespace CommonPatterns.Singleton;

/// <summary>
/// Thread-safe Singleton via <see cref="Lazy{T}"/> — the CLR guarantees the factory delegate
/// runs at most once even under concurrent first access (default <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/>).
/// </summary>
public sealed class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> LazyInstance = new(() => new AppConfiguration());

    public static AppConfiguration Instance => LazyInstance.Value;

    public IReadOnlyDictionary<string, string> Settings { get; }

    private AppConfiguration()
    {
        // Simulate an expensive one-time load (file/network/env parsing).
        Settings = new Dictionary<string, string>
        {
            ["Environment"] = "Production",
            ["MaxRetries"] = "3"
        };
    }
}
