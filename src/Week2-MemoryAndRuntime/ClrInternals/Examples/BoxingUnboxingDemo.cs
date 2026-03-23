using System.Collections;
using ClrInternals.Concepts;

namespace ClrInternals.Examples;

/// <summary>
/// Demonstrates boxing and unboxing: when value types are wrapped in
/// a heap-allocated object. Explains the allocation + copy cost and
/// how generics avoid boxing entirely.
/// </summary>
public static class BoxingUnboxingDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Boxing and Unboxing ===");

        // 1. Explicit boxing: value type → object (heap allocation + copy)
        Console.WriteLine("\n--- Explicit boxing: int → object ---");
        int value = 42;
        object boxed = value;           // box: allocates ~24 bytes on heap, copies 4 bytes
        int unboxed = (int)boxed;       // unbox: type-check + copy back to stack

        Console.WriteLine($"  int value:   {value}");
        Console.WriteLine($"  boxed:       {boxed}  (new object on heap)");
        Console.WriteLine($"  unboxed:     {unboxed}");
        // ReferenceEquals always returns false for value types — boxing creates a new object each time.
        // Suppressing CA2013: this is intentional to demonstrate the boxing behaviour.
#pragma warning disable CA2013
        Console.WriteLine($"  Same ref?    {ReferenceEquals(value, boxed)}  (always false — boxing creates new object)");
#pragma warning restore CA2013

        // 2. Interface boxing — calling IComparable on a struct boxes it
        Console.WriteLine("\n--- Interface boxing ---");
        SmallStruct p1 = new(3, 4);
        SmallStruct p2 = new(1, 2);

        // Assigning struct to interface variable → box
        Console.WriteLine("  IComparable<SmallStruct> variable = struct  → no boxing (SmallStruct doesn't implement it)");
        Console.WriteLine("  Calling a non-generic interface method on a value type DOES box.");
        Console.WriteLine($"  p1.ToString() via object: {((object)p1).ToString()}  (boxing occurred)");

        // 3. ArrayList boxes every element; List<T> does not
        Console.WriteLine("\n--- ArrayList (boxes) vs List<T> (no boxing) ---");
        long before = GC.GetTotalMemory(forceFullCollection: true);

        var arrayList = new ArrayList();
        for (int i = 0; i < 10_000; i++) arrayList.Add(i); // 10k box allocations

        long afterArrayList = GC.GetTotalMemory(forceFullCollection: false);

        var typedList = new List<int>();
        for (int i = 0; i < 10_000; i++) typedList.Add(i); // 0 box allocations

        long afterTypedList = GC.GetTotalMemory(forceFullCollection: false);

        Console.WriteLine($"  ArrayList.Add(int) ×10K: ~{afterArrayList - before:N0} bytes allocated");
        Console.WriteLine($"  List<int>.Add(int)  ×10K: ~{afterTypedList - afterArrayList:N0} bytes allocated");
        Console.WriteLine("  List<int> stores ints directly in an int[] — no boxing.");

        // 4. Struct in Dictionary<object, ...> vs Dictionary<int, ...>
        Console.WriteLine("\n--- Dictionary key boxing ---");
        var objDict = new Dictionary<object, string>();
        var intDict = new Dictionary<int, string>();

        before = GC.GetTotalMemory(forceFullCollection: true);
        for (int i = 0; i < 1000; i++) objDict[i] = "v"; // boxes each key
        long afterObjDict = GC.GetTotalMemory(forceFullCollection: false);

        for (int i = 0; i < 1000; i++) intDict[i] = "v"; // no boxing
        long afterIntDict = GC.GetTotalMemory(forceFullCollection: false);

        Console.WriteLine($"  Dictionary<object,_> ×1K: ~{afterObjDict - before:N0} bytes (keys boxed)");
        Console.WriteLine($"  Dictionary<int,_>    ×1K: ~{afterIntDict - afterObjDict:N0} bytes (no boxing)");

        // 5. How to avoid boxing: use generic constraints
        Console.WriteLine("\n--- Avoiding boxing with generics ---");
        Console.WriteLine($"  Sum via IEnumerable<object> (boxes): {SumBoxed([1, 2, 3, 4, 5])}");
        Console.WriteLine($"  Sum via generic Span<int> (no box):  {SumSpan([1, 2, 3, 4, 5])}");
    }

    private static int SumBoxed(IEnumerable<object> values)
    {
        int total = 0;
        foreach (object v in values) total += (int)v; // unbox each element
        return total;
    }

    private static int SumSpan(ReadOnlySpan<int> values)
    {
        int total = 0;
        foreach (int v in values) total += v; // no boxing
        return total;
    }
}
