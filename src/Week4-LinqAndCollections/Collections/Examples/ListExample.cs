namespace Collections.Examples;

/// <summary>
/// Demonstrates List<T> — ordered, index-accessible, mutable sequences.
/// Best default choice for general-purpose collections.
/// </summary>
public static class ListExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- List<T> ---");

        var list = new List<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Initial: {string.Join(", ", list)}");

        // Index access
        Console.WriteLine($"Index 2: {list[2]}");

        // Add
        list.Add(6);
        Console.WriteLine($"After Add(6): {string.Join(", ", list)}");

        // Insert at position
        list.Insert(2, 99);
        Console.WriteLine($"After Insert(2, 99): {string.Join(", ", list)}");

        // Remove
        list.Remove(99);
        Console.WriteLine($"After Remove(99): {string.Join(", ", list)}");

        // RemoveAt
        list.RemoveAt(0);
        Console.WriteLine($"After RemoveAt(0): {string.Join(", ", list)}");

        // Capacity
        var emptyList = new List<int>();
        Console.WriteLine($"\nEmpty list capacity: {emptyList.Capacity}");
        for (int i = 0; i < 10; i++)
        {
            emptyList.Add(i);
            if (i == 0 || i == 3 || i == 7 || i == 9)
                Console.WriteLine($"After adding {i + 1} items: capacity = {emptyList.Capacity}");
        }

        // Pre-sizing with capacity hint
        Console.WriteLine("\nWith capacity hint (capacity = 100):");
        var preSized = new List<int>(100);
        for (int i = 0; i < 100; i++)
        {
            preSized.Add(i);
        }
        Console.WriteLine($"Final capacity: {preSized.Capacity} (no resizing occurred)");

        // Range operations
        Console.WriteLine($"\nRange operations:");
        var range = list.GetRange(1, 3);
        Console.WriteLine($"GetRange(1, 3): {string.Join(", ", range)}");
    }
}
