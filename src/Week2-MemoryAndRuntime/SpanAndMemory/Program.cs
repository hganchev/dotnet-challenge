using SpanAndMemory.Examples;

Console.WriteLine("=== Week 2 — Span<T> and Memory<T> ===");

// --- Span<T> fundamentals: creation, slicing, modification ---
SpanBasicsDemo.Run();

// --- stackalloc with size guard and ArrayPool fallback ---
StackallocDemo.Run();

// --- Memory<T>: async-compatible, field-storable slicing ---
await MemoryDemo.RunAsync();

// --- ReadOnlySpan<char>: zero-alloc string operations & MemoryMarshal ---
SlicingWithoutAllocationDemo.Run();

// --- Zero-allocation parsing: CSV, query strings, IP addresses ---
ParsingDemo.Run();
