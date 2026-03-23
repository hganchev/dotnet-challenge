namespace GarbageCollector.Concepts;

/// <summary>
/// A lightweight object that should be collected in Gen0.
/// Allocates a small byte array to give the GC something to track.
/// </summary>
public class ShortLivedObject
{
    private readonly byte[] _data = new byte[64];

    public int Id { get; }

    public ShortLivedObject(int id) => Id = id;

    public override string ToString() => $"ShortLivedObject(Id={Id}, {_data.Length} bytes)";
}
