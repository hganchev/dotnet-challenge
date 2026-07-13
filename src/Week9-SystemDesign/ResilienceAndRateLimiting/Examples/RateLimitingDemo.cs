using ResilienceAndRateLimiting.RateLimiting;

namespace ResilienceAndRateLimiting.Examples;

/// <summary>Demonstrates a token-bucket and a sliding-window rate limiter side by side.</summary>
public static class RateLimitingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Rate Limiting: Token Bucket vs Sliding Window ===");

        Console.WriteLine("\n-- Token bucket (capacity 3, refill 1/sec) --");
        IRateLimiter bucket = new TokenBucketRateLimiter(capacity: 3, refillRatePerSecond: 1);
        for (var i = 1; i <= 5; i++)
        {
            Console.WriteLine($"  Request {i}: {(bucket.TryAcquire() ? "allowed" : "rejected")}");
        }

        Console.WriteLine("\n-- Sliding window (max 3 requests / 1s window) --");
        IRateLimiter window = new SlidingWindowRateLimiter(maxRequests: 3, window: TimeSpan.FromSeconds(1));
        for (var i = 1; i <= 5; i++)
        {
            Console.WriteLine($"  Request {i}: {(window.TryAcquire() ? "allowed" : "rejected")}");
        }

        Console.WriteLine();
        Console.WriteLine("Trade-off: token bucket allows short bursts up to its capacity and smooths");
        Console.WriteLine("out over time; sliding window enforces a stricter, evenly-paced limit but");
        Console.WriteLine("requires tracking every timestamp within the window.");
    }
}
