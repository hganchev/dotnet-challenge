---
applyTo: "src/Week2-MemoryAndRuntime/**"
---
# Week 2 — Memory and Runtime

## Garbage Collector
- Never call `GC.Collect()` in production code; let the GC manage collection cycles.
- Use `GC.KeepAlive()` only when demonstrating lifetime behaviour in demos — not as a pattern.
- Large objects (≥ 85 KB) go to the Large Object Heap (LOH); prefer reusing large buffers via `ArrayPool<T>`.
- Avoid finalizers unless wrapping truly unmanaged resources; use `SafeHandle` instead.
- Understand generational GC (Gen0/1/2): most objects die young in Gen0; avoid unnecessarily promoting short-lived objects to Gen2 by holding long-lived references to temporary data.
- Use `WeakReference<T>` only for caches/observers where the reference must not artificially extend the target's lifetime.

## IDisposable Pattern
- Implement `IDisposable` on any type that owns an unmanaged resource or an `IDisposable` field.
- Follow the full dispose pattern (virtual `Dispose(bool disposing)`) only when the class is unsealed and may have subclasses.
- Sealed classes with only managed `IDisposable` fields need only the simple `IDisposable` implementation.
- Always use `using` statements / `using` declarations — never call `.Dispose()` manually.
- Suppress finalizer registration in `Dispose`: `GC.SuppressFinalize(this)`.
- Throw `ObjectDisposedException` from public methods when called after disposal.
- Implement `IAsyncDisposable`/`DisposeAsync` alongside `IDisposable` when cleanup involves async I/O (e.g. flushing a network stream); consume it with `await using`.

## Span\<T\> and Memory\<T\>
- Use `Span<T>` for synchronous, stack-confined slice operations to avoid heap allocations.
- Use `Memory<T>` when the slice must survive across `await` points or be stored in a field.
- Use `stackalloc` with `Span<T>` for small, fixed-size, hot-path buffers (prefer a size guard ≤ 256 bytes).
- Prefer `MemoryMarshal` and `SequenceReader<T>` for low-level binary parsing.
- Avoid converting `Span<T>` to arrays just to pass to existing APIs; check for `ReadOnlySpan<T>` overloads first.

## CLR Internals
- Understand that value types are heap-allocated when boxed or stored in reference-type fields.
- JIT compiles IL per type argument for value-type generics (full specialisation); reference-type generics share one compiled body.
- Tiered compilation means "hot" code is re-JIT-compiled with optimisations — avoid micro-benchmarks in short-lived processes.
- Use `RuntimeHelpers.IsReferenceOrContainsReferences<T>()` to write allocation-aware generic code.
