using ClrInternals.Examples;

Console.WriteLine("=== Week 2 — CLR Internals ===");

// --- Boxing and unboxing: cost, interface boxing, generics avoid it ---
BoxingUnboxingDemo.Run();

// --- JIT compilation: tiered compilation, PrepareMethod, stack guard ---
JitCompilationDemo.Run();

// --- CLR type system: object layout, Unsafe.SizeOf, typeof vs GetType ---
TypeSystemDemo.Run();

// --- Generic specialisation: value-type bodies, IsReferenceOrContainsReferences ---
GenericSpecializationDemo.Run();
