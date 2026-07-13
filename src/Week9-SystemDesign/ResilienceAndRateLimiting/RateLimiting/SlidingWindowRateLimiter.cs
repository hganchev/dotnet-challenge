namespace ResilienceAndRateLimiting.RateLimiting;

/// <summary>Sliding window: tracks the timestamp of every request within the trailing window and
/// rejects once the count within that window reaches the limit. Stricter and more evenly-paced
/// than a token bucket, but requires remembering every timestamp in the window.</summary>
public class SlidingWindowRateLimiter : IRateLimiter
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly Queue<DateTime> _timestamps = new();
    private readonly object _lock = new();

    public SlidingWindowRateLimiter(int maxRequests, TimeSpan window)
    {
        _maxRequests = maxRequests;
        _window = window;
    }

    public bool TryAcquire()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            while (_timestamps.Count > 0 && now - _timestamps.Peek() > _window)
            {
                _timestamps.Dequeue();
            }

            if (_timestamps.Count >= _maxRequests)
            {
                return false;
            }

            _timestamps.Enqueue(now);
            return true;
        }
    }
}
