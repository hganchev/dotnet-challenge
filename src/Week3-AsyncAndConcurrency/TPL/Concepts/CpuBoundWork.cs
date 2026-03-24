namespace TPL.Concepts;

/// <summary>
/// Simulates realistic CPU-bound work items used across the TPL demos.
/// </summary>
public static class CpuBoundWork
{
    /// <summary>
    /// Computes the Nth Fibonacci number iteratively — cheap but not trivial.
    /// </summary>
    public static long Fibonacci(int n)
    {
        if (n <= 1) return n;
        long a = 0, b = 1;
        for (var i = 2; i <= n; i++)
            (a, b) = (b, a + b);
        return b;
    }

    /// <summary>
    /// Simulates a fixed-duration CPU burst using SpinWait to avoid
    /// OS thread-sleep (keeps the thread active, mimics real computation).
    /// </summary>
    public static int BusySpin(int durationMs)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(durationMs);
        var count = 0;
        while (DateTime.UtcNow < deadline)
        {
            Thread.SpinWait(1_000);
            count++;
        }
        return count;
    }

    /// <summary>
    /// Returns a work batch of the given size. Each element is an index.
    /// </summary>
    public static int[] CreateBatch(int size) => Enumerable.Range(1, size).ToArray();
}
