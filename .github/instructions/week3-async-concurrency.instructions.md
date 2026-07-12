---
applyTo: "src/Week3-AsyncAndConcurrency/**"
---
# Week 3 — Async and Concurrency

## async / await
- Always propagate `CancellationToken` through the full async call chain; don't swallow it.
- Use `ConfigureAwait(false)` in library or lower-layer code to avoid capturing the synchronisation context.
- Prefer `ValueTask<T>` over `Task<T>` for hot-path async methods that frequently complete synchronously.
- Never use `async void` except for event handlers; return `Task` or `ValueTask` instead.
- Avoid `.Result` and `.Wait()` — they block threads and cause deadlocks on synchronisation-context threads.
- Use `Task.WhenAll` / `Task.WhenAny` to compose independent async operations concurrently.
- Use `IAsyncEnumerable<T>` with `await foreach` to stream asynchronously produced sequences without buffering the whole result in memory.
- Link cancellation from multiple sources with `CancellationTokenSource.CreateLinkedTokenSource` rather than manually forwarding several tokens.

## Task Parallel Library (TPL)
- Use `Parallel.For` / `Parallel.ForEach` for CPU-bound work with independent iterations.
- Provide a `ParallelOptions` with a `MaxDegreeOfParallelism` limit to prevent thread-pool starvation.
- Prefer PLINQ (`.AsParallel()`) for data-parallel LINQ queries; remember it adds overhead — benchmark before adopting.
- Use `Task.Run` to offload CPU-bound work from the UI/request thread; do not use it inside a library.

## Thread Safety
- Protect shared mutable state with `lock` on a private `readonly object _sync = new()`.
- Use `SemaphoreSlim.WaitAsync()` instead of `lock` when the critical section spans an `await` — `lock` cannot be held across an await point.
- Prefer `Interlocked` methods for simple counter/flag operations — they are lock-free and faster than `lock`.
- Use concurrent collections (`ConcurrentDictionary<K,V>`, `ConcurrentQueue<T>`) instead of locking a plain collection.
- Prefer immutable data structures when sharing read-only state across threads — no synchronisation needed.
- Avoid `Thread.Sleep` in production code; use `Task.Delay` with a cancellation token.

## Channels
- Use `Channel.CreateBounded<T>` (with a capacity) instead of unbounded channels to apply backpressure: a bounded channel makes a producer's `WriteAsync` asynchronously wait once the channel is full, naturally slowing a fast producer down to match a slower consumer instead of letting memory grow unbounded.
- Always complete the writer (`writer.Complete()`) when producing is done so readers can drain and exit.
- Prefer `await foreach` with `channel.Reader.ReadAllAsync(ct)` over manual `WaitToReadAsync` loops.
- Use `Channel<T>` over `BlockingCollection<T>` in async pipelines — it is async-native and allocation-friendly.
