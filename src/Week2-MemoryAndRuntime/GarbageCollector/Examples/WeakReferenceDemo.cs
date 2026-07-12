namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates <see cref="WeakReference{T}"/>: a reference that lets the
/// target be garbage-collected even while the WeakReference itself is still
/// reachable. Useful for caches/observers that must never be the reason an
/// object stays alive.
/// </summary>
public static class WeakReferenceDemo
{
    private sealed class CachedItem
    {
        public string Name { get; }
        public CachedItem(string name) => Name = name;
    }

    public static void Run()
    {
        Console.WriteLine("\n=== WeakReference<T> — Not Extending an Object's Lifetime ===");

        var weakRef = CreateWeaklyReferencedItem();

        // NOTE: checking `weakRef.TryGetTarget(out var target)` yields a strong
        // local reference — if that local stayed in scope across GC.Collect(),
        // it would itself keep the object alive, defeating the demo. Isolating
        // the check in its own method ensures the strong reference dies when
        // that method returns, before we collect.
        PrintTarget("Before GC.Collect()", weakRef);

        // No other strong reference exists to the CachedItem at this point —
        // a real cache built on WeakReference<T> must be able to observe this.
        GC.Collect();
        GC.WaitForPendingFinalizers();

        PrintTarget("After GC.Collect() ", weakRef);

        // Contrast: a strong reference DOES keep the object alive across a collection.
        Console.WriteLine("\n--- Contrast: a strong reference survives GC.Collect() ---");
        var strongItem = new CachedItem("kept-alive");
        GC.Collect();
        Console.WriteLine($"  Strong reference still valid: {strongItem.Name}");
    }

    // Isolated so the strong `target` reference obtained from TryGetTarget goes
    // out of scope (and becomes collectible) as soon as this method returns.
    private static void PrintTarget(string label, WeakReference<CachedItem> weakRef)
    {
        Console.WriteLine(weakRef.TryGetTarget(out var target)
            ? $"  {label}: TryGetTarget => {target.Name}"
            : $"  {label}: TryGetTarget => collected");
    }

    // Isolated in its own method so the JIT doesn't keep a hidden strong
    // reference to `item` alive on the stack for the rest of Run().
    private static WeakReference<CachedItem> CreateWeaklyReferencedItem()
    {
        var item = new CachedItem("expensive-to-recompute");
        return new WeakReference<CachedItem>(item);
    }
}
