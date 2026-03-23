using System.Runtime.CompilerServices;
using ClrInternals.Concepts;

namespace ClrInternals.Examples;

/// <summary>
/// Demonstrates JIT generic specialisation:
/// - Value-type generics get a dedicated native code body per type argument.
/// - Reference-type generics share one native code body (pointer semantics).
///
/// Also shows <see cref="RuntimeHelpers.IsReferenceOrContainsReferences{T}"/>
/// for writing allocation-aware generic algorithms.
/// </summary>
public static class GenericSpecializationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Generic Specialisation ===");

        // 1. GenericContainer for value vs reference types
        Console.WriteLine("\n--- GenericContainer<T> instances ---");
        var intBox = new GenericContainer<int>(42);
        var doubleBox = new GenericContainer<double>(3.14);
        var stringBox = new GenericContainer<string>("hello");
        var structBox = new GenericContainer<SmallStruct>(new SmallStruct(3, 4));

        Console.WriteLine($"  {intBox}");
        Console.WriteLine($"  {doubleBox}");
        Console.WriteLine($"  {stringBox}");
        Console.WriteLine($"  {structBox}");

        // 2. Unsafe.SizeOf<T> — shows why value types are specialised
        Console.WriteLine("\n--- Unsafe.SizeOf<T> per type argument ---");
        Console.WriteLine($"  SizeOf<byte>:        {Unsafe.SizeOf<byte>()}");
        Console.WriteLine($"  SizeOf<int>:         {Unsafe.SizeOf<int>()}");
        Console.WriteLine($"  SizeOf<long>:        {Unsafe.SizeOf<long>()}");
        Console.WriteLine($"  SizeOf<SmallStruct>: {Unsafe.SizeOf<SmallStruct>()}");
        Console.WriteLine($"  SizeOf<LargeStruct>: {Unsafe.SizeOf<LargeStruct>()}");
        Console.WriteLine($"  SizeOf<string>:      {Unsafe.SizeOf<string>()} (pointer size, not string content)");
        Console.WriteLine($"  SizeOf<object>:      {Unsafe.SizeOf<object>()} (pointer size)");
        Console.WriteLine("  → Each value type gets its own JIT body because the sizes differ.");
        Console.WriteLine("  → All reference types share one JIT body: they're all pointer-sized.");

        // 3. RuntimeHelpers.IsReferenceOrContainsReferences<T>
        Console.WriteLine("\n--- RuntimeHelpers.IsReferenceOrContainsReferences<T> ---");
        PrintRefInfo<byte>("byte");
        PrintRefInfo<int>("int");
        PrintRefInfo<SmallStruct>("SmallStruct");
        PrintRefInfo<LargeStruct>("LargeStruct");
        PrintRefInfo<string>("string");
        PrintRefInfo<object>("object");
        PrintRefInfo<List<int>>("List<int>");

        // 4. Practical use: allocation-aware algorithm (like Span<T>.Clear)
        Console.WriteLine("\n--- Allocation-aware generic algorithm ---");
        Console.WriteLine("  GenericClear<T> takes a different path depending on whether T contains refs.");
        Console.WriteLine("  If T has no refs: fill with raw zero bytes (fast memset).");
        Console.WriteLine("  If T has refs:    must write null per element (GC write barriers required).");
        DemoClear<int>(new int[] { 1, 2, 3, 4, 5 });
        DemoClear<string>(new string[] { "a", "b", "c" });

        // 5. Code sharing explanation
        Console.WriteLine("\n--- JIT code sharing ---");
        Console.WriteLine("  List<string>, List<object>, List<MyClass>:");
        Console.WriteLine("    → All share ONE JIT body (all hold 8-byte pointers).");
        Console.WriteLine("  List<int>, List<double>, List<LargeStruct>:");
        Console.WriteLine("    → Each gets its OWN JIT body (different element sizes/layouts).");
        Console.WriteLine("  This is why value-type generics can have better inlining/devirtualisation.");
    }

    private static void PrintRefInfo<T>(string label)
    {
        bool hasRefs = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        Console.WriteLine($"  IsReferenceOrContainsReferences<{label,-14}> = {hasRefs}");
    }

    private static void DemoClear<T>(T[] array)
    {
        bool hasRefs = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        Console.Write($"  DemoClear<{typeof(T).Name}>[{array.Length}]: ");
        if (!hasRefs)
        {
            // No GC refs in T — safe to zero raw bytes via Array.Clear
            // (In BCL internals this would use MemoryMarshal.AsBytes via a
            //  constrained helper; here we use Array.Clear which does the same.)
            Array.Clear(array, 0, array.Length);
            Console.WriteLine("zeroed (no refs — raw memory clear).");
        }
        else
        {
            // Must null each slot so GC write barriers fire correctly
            array.AsSpan().Clear();
            Console.WriteLine("cleared element-by-element (contains refs).");
        }
    }
}
