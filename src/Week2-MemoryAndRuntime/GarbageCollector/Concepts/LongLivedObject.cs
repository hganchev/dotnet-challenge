namespace GarbageCollector.Concepts;

/// <summary>
/// An object intended to survive multiple GC cycles and be promoted to Gen2.
/// Holds a moderate buffer to represent cached or long-lived application state.
/// </summary>
public class LongLivedObject
{
    private readonly byte[] _cache = new byte[1024];

    public string Name { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public LongLivedObject(string name) => Name = name;

    public override string ToString() =>
        $"LongLivedObject(Name={Name}, Created={CreatedAt:HH:mm:ss.fff}, {_cache.Length} bytes)";
}
