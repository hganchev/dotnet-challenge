namespace ThreadSafety.Concepts;

/// <summary>
/// Demonstrates ThreadLocal&lt;T&gt; for per-thread storage — each thread gets
/// its own independent instance, so no synchronisation is needed.
/// </summary>
public static class ThreadLocalDemo
{
    // Each thread gets its own request-scoped "context" string.
    private static readonly ThreadLocal<string> s_requestContext =
        new(() => $"[no context – thread {Thread.CurrentThread.ManagedThreadId}]");

    public static string CurrentContext
    {
        get => s_requestContext.Value!;
        set => s_requestContext.Value = value;
    }

    public static void Run()
    {
        Console.WriteLine("--- ThreadLocal<T> per-thread storage ---");

        var t1 = new Thread(() =>
        {
            CurrentContext = "Request-A";
            Thread.Sleep(20);
            Console.WriteLine($"  Thread 1: {CurrentContext}");
        });

        var t2 = new Thread(() =>
        {
            CurrentContext = "Request-B";
            Thread.Sleep(10);
            Console.WriteLine($"  Thread 2: {CurrentContext}");
        });

        t1.Start(); t2.Start();
        t1.Join();  t2.Join();

        Console.WriteLine("  (Each thread has its own isolated value — no lock needed)\n");
    }
}
