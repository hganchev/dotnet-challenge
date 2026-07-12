# Week 2 - Memory and Runtime

## Topics
- **GarbageCollector** – GC generations, finalization, GC.Collect, large object heap
- **IDisposablePattern** – IDisposable, using statement, finalizers, SafeHandle
- **SpanAndMemory** – Span\<T\>, Memory\<T\>, stackalloc, slicing without allocation
- **ClrInternals** – CLR architecture, JIT compilation, type system internals

## Implementation Plan

### Implementation Order
| Step | Project              | Why                                                      |
|------|----------------------|----------------------------------------------------------|
| 1    | GarbageCollector     | Foundation — understand heap, generations, finalization   |
| 2    | IDisposablePattern   | Builds on GC knowledge — deterministic cleanup            |
| 3    | SpanAndMemory        | Applies memory awareness — stack vs heap allocation       |
| 4    | ClrInternals         | Ties it all together — JIT, type system, boxing           |

### 1. GarbageCollector
**Folder structure:**
```
GarbageCollector/
  Program.cs                          # Orchestrator — calls all demos
  Examples/
    GenerationsDemo.cs                # GC.GetGeneration(), promote objects across Gen0→1→2
    FinalizationDemo.cs               # ~Finalizer, GC.WaitForPendingFinalizers
    GcCollectDemo.cs                  # GC.Collect() behaviour, memory pressure
    LargeObjectHeapDemo.cs            # LOH allocation (≥85KB), ArrayPool<T> reuse
  Concepts/
    ShortLivedObject.cs               # Lightweight class to show Gen0 collection
    LongLivedObject.cs                # Object promoted to Gen2
    FinalizableResource.cs            # Class with finalizer for demo
```
**Key demos:** GC generations (Gen0/1/2), finalizer queue, `GC.Collect()` + `GC.GetTotalMemory()`, LOH threshold, `ArrayPool<T>` as alternative.

### 2. IDisposablePattern
**Folder structure:**
```
IDisposablePattern/
  Program.cs
  Examples/
    SimpleDisposeDemo.cs              # Sealed class, simple IDisposable
    FullDisposePatternDemo.cs         # Unsealed class with Dispose(bool) + finalizer
    UsingStatementDemo.cs             # using statement & using declaration
    SafeHandleDemo.cs                 # SafeHandle wrapping unmanaged resource
    ObjectDisposedDemo.cs             # Throwing ObjectDisposedException
  Disposables/
    ManagedResourceHolder.cs          # Sealed — owns only IDisposable fields
    UnmanagedResourceHolder.cs        # Unsealed — full dispose pattern
    DerivedResourceHolder.cs          # Subclass overriding Dispose(bool)
    FileHandleWrapper.cs              # SafeHandle example
```
**Key demos:** Simple vs full dispose pattern, `using` statement/declaration, `GC.SuppressFinalize`, `SafeHandle`, `ObjectDisposedException`.

### 3. SpanAndMemory
**Folder structure:**
```
SpanAndMemory/
  Program.cs
  Examples/
    SpanBasicsDemo.cs                 # Creating Span<T>, slicing, iteration
    StackallocDemo.cs                 # stackalloc with Span<T>, size guard
    MemoryDemo.cs                     # Memory<T> for async-compatible slicing
    SlicingWithoutAllocationDemo.cs   # String slicing via ReadOnlySpan<char>
    ParsingDemo.cs                    # Parsing CSV/numbers without allocation
  Concepts/
    SpanHelper.cs                     # Utility showing Span<T> operations
    MemoryOwnerExample.cs             # IMemoryOwner<T> / MemoryPool<T>
```
**Key demos:** `Span<T>` vs arrays, `stackalloc` with ≤256-byte guard, `Memory<T>` across await, `ReadOnlySpan<char>` for zero-alloc string parsing, `MemoryMarshal`.

### 4. ClrInternals
**Folder structure:**
```
ClrInternals/
  Program.cs
  Examples/
    BoxingUnboxingDemo.cs             # Value type boxing, perf impact
    JitCompilationDemo.cs             # Tiered compilation, method prep
    TypeSystemDemo.cs                 # Object header, method table, typeof
    GenericSpecializationDemo.cs      # Value-type vs reference-type generic code sharing
  Concepts/
    SmallStruct.cs                    # Value type for boxing demos
    LargeStruct.cs                    # Struct exceeding copy-cost threshold
    GenericContainer.cs               # Generic class to show specialization
```
**Key demos:** Boxing/unboxing cost, JIT tiered compilation timing, `RuntimeHelpers.IsReferenceOrContainsReferences<T>()`, value-type generic specialization vs reference-type sharing.

---

## Notes

### Garbage Collector
- .NET uses a **generational GC** with three generations: Gen0 (short-lived), Gen1 (buffer), Gen2 (long-lived)
- Objects ≥ 85KB are allocated on the **Large Object Heap (LOH)**, collected only with Gen2
- **Finalizers** (`~ClassName`) delay collection — object survives at least one extra GC cycle
- `GC.Collect()` forces collection — avoid in production; useful for learning/benchmarks
- `ArrayPool<T>.Shared` reuses large buffers instead of repeated LOH allocations
- `GC.GetGeneration(obj)` reveals which generation an object currently resides in
- `GC.GetTotalMemory(forceFullCollection)` reports approximate managed heap size
- `WeakReference<T>` lets a cache/observer reference an object without keeping it alive — see `GarbageCollector/Examples/WeakReferenceDemo.cs`. Gotcha found while implementing it: calling `TryGetTarget(out var target)` yields a **strong** local reference; if that local stays in scope across a later `GC.Collect()`, it defeats the demo. Isolate the check in its own method so the strong reference dies when that method returns.

### IDisposable Pattern
- **Simple pattern** (sealed class): implement `IDisposable.Dispose()` directly
- **Full pattern** (unsealed class): virtual `Dispose(bool disposing)` + finalizer safety net
- `using` statement / `using` declaration ensures `Dispose()` is called even on exceptions
- `GC.SuppressFinalize(this)` in `Dispose()` prevents unnecessary finalization
- `SafeHandle` is preferred over raw `IntPtr` for wrapping unmanaged handles
- Throw `ObjectDisposedException` from public methods after disposal
- Implement `IAsyncDisposable`/`DisposeAsync` alongside `IDisposable` when cleanup involves async I/O; consume with `await using` — see `IDisposablePattern/Disposables/AsyncResourceHolder.cs` and `Examples/AsyncDisposableDemo.cs`.

### Span and Memory
- `Span<T>` is a stack-only ref struct — cannot be stored in fields or used across `await`
- `Memory<T>` is the heap-safe counterpart for async scenarios
- `stackalloc` allocates on the stack — fast, no GC pressure, but limited size (guard ≤ 256 bytes)
- `ReadOnlySpan<char>` enables zero-allocation string slicing (`"hello".AsSpan().Slice(1, 3)`)
- `MemoryPool<T>.Shared` provides pooled `IMemoryOwner<T>` for reusable buffers

### CLR Internals
- **Boxing** wraps a value type in a heap object — has allocation + copy cost
- **JIT** compiles IL → native code per method on first call; tiered compilation re-optimises hot paths
- Value-type generics get **full specialisation** (separate native code per type argument)
- Reference-type generics **share** one compiled body (code sharing)
- `RuntimeHelpers.IsReferenceOrContainsReferences<T>()` helps write allocation-aware generic code
