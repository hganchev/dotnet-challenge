# Week 3 - Async and Concurrency

## Goal
Master asynchronous and concurrent programming in .NET: how async/await works under the hood, how to parallelise CPU-bound work with TPL, how to protect shared state from race conditions, and how to build efficient producer/consumer pipelines with `System.Threading.Channels`.

---

## 1. Async / Await

### Topics to Cover
| Topic | Key Points |
|---|---|
| `async` / `await` basics | State-machine transformation; `Task` and `Task<T>` return types |
| `ValueTask<T>` | Allocation-free hot-path async; when to prefer over `Task<T>` |
| `ConfigureAwait(false)` | Avoid capturing sync context in library/lower-layer code |
| `CancellationToken` | Propagate through the full call chain; `OperationCanceledException` |
| `Task.WhenAll` / `Task.WhenAny` | Compose independent operations concurrently |
| `async void` pitfall | Only for event handlers; unhandled exceptions crash the process |
| Avoiding `.Result` / `.Wait()` | Deadlock risk on sync-context threads; use `await` instead |
| Exception handling | `try/catch` around `await`; aggregate exceptions with `WhenAll` |
| `IAsyncEnumerable<T>` | `await foreach` for async streams; `yield return` in async iterators |

### Expected Outcomes
- Explain what the compiler generates for an `async` method (state machine)
- Identify and fix a deadlock caused by blocking on `Task.Result` inside an `async` method
- Write an async pipeline that fans out work with `Task.WhenAll` and handles partial failures

---

## 2. Task Parallel Library (TPL)

### Topics to Cover
| Topic | Key Points |
|---|---|
| `Task.Run` | Offload CPU-bound work to thread pool; don't use inside library code |
| `Parallel.For` / `Parallel.ForEach` | Data parallelism for independent iterations; provide `ParallelOptions` |
| `MaxDegreeOfParallelism` | Prevent thread-pool starvation; size to `Environment.ProcessorCount` |
| PLINQ (`.AsParallel()`) | Parallel LINQ queries; `AsOrdered()`, `WithDegreeOfParallelism()` |
| `Task.ContinueWith` | Chain task continuations; prefer `await` over this in modern code |
| `Parallel.Invoke` | Run a fixed set of actions in parallel |
| `TaskCompletionSource<T>` | Bridge callback-based APIs to the Task model |
| Exception aggregation | `AggregateException` in `Task`-based APIs; `await` unwraps the first |

### Expected Outcomes
- Choose between `Task.Run` and `Parallel.ForEach` for a given CPU-bound problem
- Limit parallelism correctly to avoid thread-pool saturation
- Explain the difference between data parallelism (PLINQ / `Parallel.ForEach`) and task parallelism (`Task.WhenAll`)

---

## 3. Thread Safety

### Topics to Cover
| Topic | Key Points |
|---|---|
| `lock` statement | Mutual exclusion on a private `readonly object _sync = new()` |
| `Monitor` | Lower-level API behind `lock`; `Monitor.Wait` / `Pulse` for signalling |
| `Interlocked` | Lock-free atomic operations: `Increment`, `Decrement`, `CompareExchange` |
| `volatile` | Visibility guarantee; weaker than `Interlocked`; rarely sufficient alone |
| `ConcurrentDictionary<K,V>` | Thread-safe map; `GetOrAdd`, `AddOrUpdate` with factory delegates |
| `ConcurrentQueue<T>` / `ConcurrentBag<T>` | Thread-safe FIFO and unordered collections |
| `ReaderWriterLockSlim` | Multiple concurrent readers; exclusive writes |
| `SemaphoreSlim` | Async-compatible throttling (`WaitAsync` / `Release`) |
| Immutability | Sharing read-only state across threads — no synchronisation needed |
| `ThreadLocal<T>` | Per-thread storage for context data |
| `Task.Delay` vs `Thread.Sleep` | Prefer `Task.Delay` in async code; `Thread.Sleep` blocks the thread |

### Expected Outcomes
- Identify race conditions in shared mutable state and choose the right fix
- Use `Interlocked` for a high-frequency counter instead of `lock`
- Implement an async throttle with `SemaphoreSlim`

---

## 4. Channels

### Topics to Cover
| Topic | Key Points |
|---|---|
| `Channel.CreateBounded<T>` | Bounded capacity applies back-pressure to producers |
| `Channel.CreateUnbounded<T>` | Unbounded; use only when the producer rate is known to be safe |
| `ChannelWriter<T>` | Write items; `TryWrite`, `WriteAsync`, `Complete()` when done |
| `ChannelReader<T>` | Consume items; `ReadAsync`, `ReadAllAsync`, `WaitToReadAsync` |
| `await foreach` | Drain reader with `ReadAllAsync(ct)` — clean and cancellation-aware |
| Multi-producer / multi-consumer | Multiple writers and readers on the same channel |
| Pipelines | Chain channels to form processing stages |
| `Channel<T>` vs `BlockingCollection<T>` | `Channel<T>` is async-native and allocation-friendly; prefer it |

### Expected Outcomes
- Build a bounded producer/consumer pipeline with at least two stages
- Explain why `writer.Complete()` must be called and what happens without it
- Compare `Channel<T>` with `BlockingCollection<T>` and `ConcurrentQueue<T>` for async scenarios

---

## Implementation Plan

### Implementation Order
| Step | Project | Why |
|---|---|---|
| 1 | AsyncAwait | Foundation — understand state machines, cancellation, composition |
| 2 | TPL | CPU-bound parallelism builds on task fundamentals |
| 3 | ThreadSafety | Shared-state protection; bridges sync and async worlds |
| 4 | Channels | Async pipelines tie together async and concurrency concepts |

### 1. AsyncAwait
**Folder structure:**
```
AsyncAwait/
  Program.cs                            # Orchestrator — calls all demos
  Examples/
    BasicAsyncDemo.cs                   # async/await, Task<T>, exception handling
    ConfigureAwaitDemo.cs               # ConfigureAwait(false) in library code
    CancellationDemo.cs                 # CancellationToken propagation, cooperative cancellation
    WhenAllWhenAnyDemo.cs               # Fan-out with Task.WhenAll / Task.WhenAny
    AsyncStreamDemo.cs                  # IAsyncEnumerable<T>, await foreach
    ValueTaskDemo.cs                    # ValueTask<T> for hot-path methods
  Concepts/
    AsyncStateMachineExplained.cs       # Commentary-heavy file showing generated state machine
```
**Key demos:** async state machine, `ConfigureAwait`, `CancellationTokenSource`, `Task.WhenAll` with partial failure, `IAsyncEnumerable<T>`, `ValueTask<T>`.

### 2. TPL
**Folder structure:**
```
TPL/
  Program.cs
  Examples/
    TaskRunDemo.cs                      # Task.Run, fire-and-forget pitfalls
    ParallelForDemo.cs                  # Parallel.For / ForEach with ParallelOptions
    PlinqDemo.cs                        # .AsParallel(), AsOrdered(), WithDegreeOfParallelism
    ParallelInvokeDemo.cs               # Parallel.Invoke for fixed action sets
    TaskCompletionSourceDemo.cs         # Bridge callback APIs to Task model
    AggregateExceptionDemo.cs           # Exception handling with Task.WhenAll
  Concepts/
    CpuBoundWork.cs                     # Simulated CPU work for parallel demos
```
**Key demos:** `Parallel.ForEach` with `MaxDegreeOfParallelism`, PLINQ ordering, `TaskCompletionSource<T>`, `AggregateException` handling.

### 3. ThreadSafety
**Folder structure:**
```
ThreadSafety/
  Program.cs
  Examples/
    LockDemo.cs                         # lock statement, Monitor.Wait/Pulse
    InterlockedDemo.cs                  # Interlocked.Increment, CompareExchange
    ConcurrentCollectionsDemo.cs        # ConcurrentDictionary, ConcurrentQueue
    SemaphoreSlimDemo.cs                # Async throttling with SemaphoreSlim
    ReaderWriterDemo.cs                 # ReaderWriterLockSlim for read-heavy scenarios
    ImmutabilityDemo.cs                 # Sharing immutable state without locks
  Concepts/
    SharedCounter.cs                    # Counter with lock vs Interlocked comparison
    ThreadLocalDemo.cs                  # ThreadLocal<T> per-thread context
```
**Key demos:** Race condition + fix, `Interlocked` counter, `ConcurrentDictionary.GetOrAdd`, `SemaphoreSlim.WaitAsync`, `ReaderWriterLockSlim`, immutable record sharing.

### 4. Channels
**Folder structure:**
```
Channels/
  Program.cs
  Examples/
    BasicChannelDemo.cs                 # CreateBounded, WriteAsync, ReadAllAsync
    ProducerConsumerDemo.cs             # Single producer / single consumer
    MultiProducerConsumerDemo.cs        # Multiple producers and consumers
    PipelineDemo.cs                     # Chained channels forming a two-stage pipeline
    BackPressureDemo.cs                 # Bounded channel blocking producer on full
  Concepts/
    ChannelStage.cs                     # Reusable pipeline stage abstraction
```
**Key demos:** Bounded vs unbounded channels, `writer.Complete()` + drain, multi-producer fan-in, multi-stage pipeline, back-pressure demonstration.

---

## Notes

### Async / Await
- `async` methods compile to a **state machine**; `await` suspends the method and registers a continuation
- Use `ConfigureAwait(false)` in library or lower-layer code to avoid capturing the synchronisation context
- Prefer `ValueTask<T>` over `Task<T>` for hot-path async methods that frequently complete synchronously
- Never use `async void` except for event handlers — unhandled exceptions crash the process
- Avoid `.Result` and `.Wait()` — they block threads and can cause deadlocks on sync-context threads
- Use `Task.WhenAll` / `Task.WhenAny` to compose independent async operations concurrently
- Always propagate `CancellationToken` through the full async call chain; don't swallow it
- `IAsyncEnumerable<T>` + `await foreach` enables streaming async sequences without buffering all results

### Task Parallel Library (TPL)
- Use `Parallel.For` / `Parallel.ForEach` for **CPU-bound** work with independent iterations
- Provide a `ParallelOptions` with `MaxDegreeOfParallelism` to prevent thread-pool starvation
- Prefer PLINQ (`.AsParallel()`) for data-parallel LINQ queries — benchmark first, it adds overhead
- Use `Task.Run` to offload CPU-bound work from the UI/request thread; don't use it inside a library
- `AggregateException` wraps multiple exceptions from parallel/task operations; `await` unwraps the first
- `TaskCompletionSource<T>` bridges callback-based or event-based APIs into the `Task` model

### Thread Safety
- Protect shared mutable state with `lock` on a `private readonly object _sync = new()`
- Prefer `Interlocked` for simple counter/flag operations — lock-free and faster than `lock`
- Use concurrent collections (`ConcurrentDictionary<K,V>`, `ConcurrentQueue<T>`) instead of locking a plain collection
- `SemaphoreSlim` is the async-compatible throttle — use `WaitAsync` / `Release` in async code
- `ReaderWriterLockSlim` improves throughput when reads vastly outnumber writes
- Prefer immutable data structures for read-only shared state — no synchronisation needed
- Avoid `Thread.Sleep` in production; use `Task.Delay` with a cancellation token

### Channels
- Use `Channel.CreateBounded<T>` (with a capacity) to apply back-pressure on producers
- Always call `writer.Complete()` when producing is done so readers can drain and exit cleanly
- Prefer `await foreach` with `channel.Reader.ReadAllAsync(ct)` over manual `WaitToReadAsync` loops
- Use `Channel<T>` over `BlockingCollection<T>` in async pipelines — it is async-native and allocation-friendly
- Chain channels to form multi-stage pipelines where each stage consumes from one channel and writes to the next

---

## Coverage Audit

### Async / Await
| Topic | Covered? | File |
|---|---|---|
| async/await state machine | ✅ | `AsyncAwait/Concepts/AsyncStateMachineExplained.cs` |
| `ConfigureAwait(false)` | ✅ | `AsyncAwait/Examples/ConfigureAwaitDemo.cs` |
| `CancellationToken` propagation | ✅ | `AsyncAwait/Examples/CancellationDemo.cs` |
| `Task.WhenAll` / `Task.WhenAny` | ✅ | `AsyncAwait/Examples/WhenAllWhenAnyDemo.cs` |
| `async void` pitfall | ✅ | `AsyncAwait/Examples/BasicAsyncDemo.cs` (documented in notes) |
| `ValueTask<T>` | ✅ | `AsyncAwait/Examples/ValueTaskDemo.cs` |
| `IAsyncEnumerable<T>` + `await foreach` | ✅ | `AsyncAwait/Examples/AsyncStreamDemo.cs` |
| Exception handling in async | ✅ | `AsyncAwait/Examples/BasicAsyncDemo.cs`, `WhenAllWhenAnyDemo.cs` |

### TPL
| Topic | Covered? | File |
|---|---|---|
| `Task.Run` | ⬜ | `TPL/` – not yet implemented |
| `Parallel.For` / `ForEach` + `ParallelOptions` | ⬜ | `TPL/` – not yet implemented |
| `MaxDegreeOfParallelism` | ⬜ | `TPL/` – not yet implemented |
| PLINQ `.AsParallel()` | ⬜ | `TPL/` – not yet implemented |
| `Parallel.Invoke` | ⬜ | `TPL/` – not yet implemented |
| `TaskCompletionSource<T>` | ⬜ | `TPL/` – not yet implemented |
| `AggregateException` handling | ⬜ | `TPL/` – not yet implemented |

### Thread Safety
| Topic | Covered? | File |
|---|---|---|
| `lock` + `Monitor` | ⬜ | `ThreadSafety/` – not yet implemented |
| `Interlocked` operations | ⬜ | `ThreadSafety/` – not yet implemented |
| `ConcurrentDictionary<K,V>` + `ConcurrentQueue<T>` | ⬜ | `ThreadSafety/` – not yet implemented |
| `SemaphoreSlim` async throttling | ⬜ | `ThreadSafety/` – not yet implemented |
| `ReaderWriterLockSlim` | ⬜ | `ThreadSafety/` – not yet implemented |
| Immutable data sharing | ⬜ | `ThreadSafety/` – not yet implemented |
| `ThreadLocal<T>` | ⬜ | `ThreadSafety/` – not yet implemented |

### Channels
| Topic | Covered? | File |
|---|---|---|
| `Channel.CreateBounded<T>` + back-pressure | ⬜ | `Channels/` – not yet implemented |
| `ChannelWriter<T>` + `Complete()` | ⬜ | `Channels/` – not yet implemented |
| `ChannelReader<T>` + `ReadAllAsync` | ⬜ | `Channels/` – not yet implemented |
| `await foreach` drain | ⬜ | `Channels/` – not yet implemented |
| Multi-producer / multi-consumer | ⬜ | `Channels/` – not yet implemented |
| Multi-stage pipeline | ⬜ | `Channels/` – not yet implemented |

### Gaps to Address
- ~~Implement AsyncAwait project~~ ✅ Done
- Implement TPL project: `Parallel.ForEach` with `ParallelOptions`, PLINQ, `TaskCompletionSource`, exception aggregation
- Implement ThreadSafety project: race condition demo + fix, `Interlocked`, concurrent collections, `SemaphoreSlim`, `ReaderWriterLockSlim`
- Implement Channels project: bounded channel with back-pressure, producer/consumer, multi-stage pipeline
