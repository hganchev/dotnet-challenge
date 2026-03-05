namespace Generics.TypeConstraints;

// where T : IComparable<T> — T must implement comparison
// Enables: calling .CompareTo() inside generic methods
public class SortedBag<T> where T : IComparable<T>
{
    private readonly List<T> _items = [];

    public void Add(T item)
    {
        _items.Add(item);
        _items.Sort(); // valid because T : IComparable<T>
    }

    public T Min() => _items.Count > 0 ? _items[0] : throw new InvalidOperationException("Bag is empty.");
    public T Max() => _items.Count > 0 ? _items[^1] : throw new InvalidOperationException("Bag is empty.");

    public IReadOnlyList<T> Items => _items.AsReadOnly();
}

public static class InterfaceConstraintExamples
{
    // Generic binary search — requires ordered elements
    public static int BinarySearch<T>(T[] sorted, T target) where T : IComparable<T>
    {
        int lo = 0, hi = sorted.Length - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            int cmp = sorted[mid].CompareTo(target);
            if (cmp == 0) return mid;
            if (cmp < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return -1;
    }

    public static void Run()
    {
        Console.WriteLine("\n=== where T : IComparable<T> ===");

        var bag = new SortedBag<int>();
        foreach (var n in new[] { 5, 2, 8, 1, 9 })
            bag.Add(n);

        Console.WriteLine($"Sorted: [{string.Join(", ", bag.Items)}]");
        Console.WriteLine($"Min: {bag.Min()}, Max: {bag.Max()}");

        int[] arr = [1, 3, 5, 7, 9, 11];
        Console.WriteLine($"BinarySearch(7) index: {BinarySearch(arr, 7)}");
        Console.WriteLine($"BinarySearch(6) index: {BinarySearch(arr, 6)} (not found)");
    }
}
