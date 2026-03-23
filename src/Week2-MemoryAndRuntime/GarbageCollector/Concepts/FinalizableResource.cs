namespace GarbageCollector.Concepts;

/// <summary>
/// Demonstrates a class with a finalizer (~destructor).
/// The finalizer delays collection: the object survives at least one extra GC cycle
/// because it must be moved to the finalization queue first.
/// </summary>
public class FinalizableResource
{
    private static int _instanceCount;

    public int InstanceId { get; }
    public string Label { get; }

    public FinalizableResource(string label)
    {
        InstanceId = Interlocked.Increment(ref _instanceCount);
        Label = label;
        Console.WriteLine($"  [Finalizable] Created #{InstanceId} '{Label}'");
    }

    ~FinalizableResource()
    {
        Console.WriteLine($"  [Finalizable] Finalized #{InstanceId} '{Label}'");
    }

    /// <summary>
    /// Resets the static counter — useful between demo runs.
    /// </summary>
    public static void ResetCounter() => Interlocked.Exchange(ref _instanceCount, 0);

    public override string ToString() => $"FinalizableResource(#{InstanceId} '{Label}')";
}
