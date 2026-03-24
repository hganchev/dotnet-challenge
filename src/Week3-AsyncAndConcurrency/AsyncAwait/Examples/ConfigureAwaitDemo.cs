namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates why and when to use ConfigureAwait(false).
/// Library code should avoid capturing the synchronisation context so it
/// does not deadlock callers that run on a context (e.g. ASP.NET Classic,
/// WinForms, WPF).
/// </summary>
public static class ConfigureAwaitDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== ConfigureAwait Demo ===\n");

        await ShowContextBehaviourAsync();
        await ShowLibraryPatternAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Default behaviour — continuation captures current SynchronisationContext
    //    In a console app there is no sync context, so both behave the same.
    //    In a UI / ASP.NET Classic app, ConfigureAwait(true) (default) marshals
    //    the continuation back to the original thread/context.
    // -------------------------------------------------------------------------
    private static async Task ShowContextBehaviourAsync()
    {
        Console.WriteLine("--- SynchronisationContext ---");
        Console.WriteLine($"Context before await : {SynchronizationContext.Current?.GetType().Name ?? "null (thread-pool)"}");

        await Task.Delay(10); // ConfigureAwait(true) by default

        Console.WriteLine($"Context after await (default)           : {SynchronizationContext.Current?.GetType().Name ?? "null (thread-pool)"}");

        await Task.Delay(10).ConfigureAwait(false);

        Console.WriteLine($"Context after await (ConfigureAwait(false)): {SynchronizationContext.Current?.GetType().Name ?? "null (thread-pool)"}");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. Library pattern — every await in library code uses ConfigureAwait(false)
    //    Rule: ConfigureAwait(false) in any code that is NOT a UI event handler
    //          or an ASP.NET Core request handler.
    // -------------------------------------------------------------------------
    private static async Task ShowLibraryPatternAsync()
    {
        Console.WriteLine("--- Library Pattern ---");
        var result = await LibraryMethodAsync("payload").ConfigureAwait(false);
        Console.WriteLine($"Library returned: {result}");
        Console.WriteLine();
    }

    /// <summary>
    /// Simulates a reusable library method.
    /// Every internal await uses ConfigureAwait(false) to avoid context
    /// capture and prevent deadlocks when called from a synchronisation-context thread.
    /// </summary>
    private static async Task<string> LibraryMethodAsync(string input)
    {
        // Step 1 — I/O simulation; do NOT capture context
        await Task.Delay(20).ConfigureAwait(false);

        var intermediate = input.ToUpperInvariant();

        // Step 2 — another I/O hop; still no context capture
        await Task.Delay(10).ConfigureAwait(false);

        return $"processed:{intermediate}";
    }
}
