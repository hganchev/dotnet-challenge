namespace ValueVsReference.Examples;

// Boxing:   wraps a value type in a heap-allocated object reference.
// Unboxing: extracts the value type back from the boxed object.
// Both incur allocation + copy overhead — avoid in hot paths.

public static class BoxingUnboxingDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Boxing and Unboxing ===");

        // --- Implicit boxing: int -> object ---
        int number = 42;
        object boxed = number;          // heap allocation happens here
        Console.WriteLine($"boxed type: {boxed.GetType()}, value: {boxed}");

        // Mutating the original does NOT affect the boxed copy
        number = 100;
        Console.WriteLine($"number={number}, boxed still={boxed}");

        // --- Explicit unboxing: object -> int ---
        int unboxed = (int)boxed;       // must cast to exact type
        Console.WriteLine($"unboxed: {unboxed}");

        // --- Wrong unbox type throws InvalidCastException ---
        try
        {
            long wrong = (long)boxed;   // boxed holds System.Int32, not Int64
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"InvalidCastException: {ex.Message}");
        }

        // --- Boxing with interfaces ---
        // Assigning a struct to an interface causes boxing
        IComparable comparable = 7;     // int is boxed
        Console.WriteLine($"IComparable.CompareTo(10): {comparable.CompareTo(10)}");

        // --- Performance cost: boxing in a non-generic collection ---
        var arrayList = new System.Collections.ArrayList();
        for (int i = 0; i < 5; i++)
            arrayList.Add(i);           // each Add boxes the int

        int sum = 0;
        foreach (object item in arrayList)
            sum += (int)item;           // each access unboxes
        Console.WriteLine($"ArrayList sum (boxed): {sum}");

        // --- Prefer generic collections to avoid boxing ---
        var list = new List<int>();
        for (int i = 0; i < 5; i++)
            list.Add(i);                // no boxing

        int sumGeneric = 0;
        foreach (int item in list)
            sumGeneric += item;
        Console.WriteLine($"List<int> sum (no boxing): {sumGeneric}");
    }
}
