namespace ThreadSafety.Concepts;

/// <summary>
/// A counter implemented three ways — unsynchronised, lock-based, and
/// Interlocked — so demos can compare safety and performance.
/// </summary>
public sealed class SharedCounter
{
    // -------------------------------------------------------------------------
    // Unsynchronised — deliberately racy; for demo purposes only
    // -------------------------------------------------------------------------
    private int _unsafeValue;

    public void UnsafeIncrement() => _unsafeValue++;
    public int UnsafeValue => _unsafeValue;

    // -------------------------------------------------------------------------
    // Lock-based — safe on all platforms; preferred for complex invariants
    // -------------------------------------------------------------------------
    private readonly object _sync = new();
    private int _lockedValue;

    public void LockedIncrement()
    {
        lock (_sync) { _lockedValue++; }
    }
    public int LockedValue => _lockedValue;

    // -------------------------------------------------------------------------
    // Interlocked — lock-free atomic; preferred for simple counters
    // -------------------------------------------------------------------------
    private int _atomicValue;

    public void AtomicIncrement() => Interlocked.Increment(ref _atomicValue);
    public int AtomicValue => _atomicValue;
}
