using ValueVsReference.ValueTypes;

namespace ValueVsReference.Examples;

// Value types live on the stack (or inline inside containing type on the heap).
// Assignment copies the entire value — no shared state between variables.

public static class ValueTypeBasics
{
    public static void Run()
    {
        Console.WriteLine("\n=== Value Types ===");

        // --- Primitives: copy semantics ---
        int a = 10;
        int b = a;   // b gets an independent copy
        b = 99;
        Console.WriteLine($"a={a}, b={b}");  // a is still 10

        // --- Struct: copy semantics ---
        var p1 = new Point(1, 2);
        var p2 = p1;   // full copy
        p2.X = 100;
        Console.WriteLine($"p1={p1}, p2={p2}");  // p1 unchanged

        // --- Struct mutation requires ref ---
        MutatePoint(ref p1);
        Console.WriteLine($"p1 after ref mutation: {p1}");  // X changed

        // --- Enum is a value type ---
        Direction dir = Direction.North;
        Console.WriteLine($"Direction: {dir} (underlying int: {(int)dir})");

        // --- sizeof ---
        Console.WriteLine($"sizeof(int)={sizeof(int)}, sizeof(double)={sizeof(double)}, sizeof(bool)={sizeof(bool)}");
    }

    // Without 'ref', the struct is copied — the caller's value is unaffected.
    // With 'ref', the method operates on the same memory location.
    private static void MutatePoint(ref Point p)
    {
        p.X = 42;
    }
}
