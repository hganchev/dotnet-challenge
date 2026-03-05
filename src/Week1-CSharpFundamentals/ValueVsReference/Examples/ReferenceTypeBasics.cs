using ValueVsReference.ReferenceTypes;

namespace ValueVsReference.Examples;

// Reference types are allocated on the heap.
// A variable holds a *reference* (pointer) to the object, not the object itself.
// Assignment copies the reference — both variables point to the same object.

public static class ReferenceTypeBasics
{
    public static void Run()
    {
        Console.WriteLine("\n=== Reference Types ===");

        // --- Class: reference (alias) semantics ---
        var personA = new Person("Alice", 30);
        var personB = personA;    // both variables point to the same heap object
        personB.Name = "Bob";
        Console.WriteLine($"personA.Name={personA.Name}");  // Bob — shared object mutated

        // --- null is the default for reference types ---
        Person? nobody = null;
        Console.WriteLine($"nobody is null: {nobody is null}");

        // --- Arrays are reference types ---
        int[] arr1 = { 1, 2, 3 };
        int[] arr2 = arr1;          // same array on the heap
        arr2[0] = 99;
        Console.WriteLine($"arr1[0]={arr1[0]}");  // 99 — shared reference

        // Independent copy requires Array.Copy / ToArray
        int[] arr3 = arr1.ToArray();
        arr3[0] = 0;
        Console.WriteLine($"arr1[0] after arr3 mutation={arr1[0]}");  // still 99

        // --- string is a reference type with VALUE-like semantics (immutable + interning) ---
        string s1 = "hello";
        string s2 = "hello";
        Console.WriteLine($"String ReferenceEquals: {ReferenceEquals(s1, s2)}");  // true — interned
        // new string(span) forces a fresh heap allocation (not interned)
        string s3 = new string(s1.AsSpan());
        Console.WriteLine($"New string ReferenceEquals: {ReferenceEquals(s1, s3)}");  // false
        Console.WriteLine($"String == : {s1 == s3}");  // true — value equality via overloaded ==
    }
}
