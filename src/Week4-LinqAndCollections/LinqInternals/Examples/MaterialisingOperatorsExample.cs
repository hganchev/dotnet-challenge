namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates materialising operators (ToList, ToArray, ToDictionary) that force
/// eager evaluation of the entire sequence.
/// </summary>
public static class MaterialisingOperatorsExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Materialising Operators ---");

        var numbers = Enumerable.Range(1, 10);

        // ToList()
        Console.WriteLine("ToList():");
        var list = numbers
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToList();
        Console.WriteLine($"  Type: {list.GetType().Name}, Count: {list.Count}, Items: {string.Join(", ", list)}");

        // ToArray()
        Console.WriteLine("\nToArray():");
        var array = numbers
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToArray();
        Console.WriteLine($"  Type: {array.GetType().Name}, Length: {array.Length}, Items: {string.Join(", ", array)}");

        // ToDictionary()
        Console.WriteLine("\nToDictionary():");
        var people = new[] { (Id: 1, Name: "Alice"), (Id: 2, Name: "Bob"), (Id: 3, Name: "Charlie") };
        var dict = people.ToDictionary(p => p.Id, p => p.Name);
        Console.WriteLine($"  Type: {dict.GetType().Name}, Count: {dict.Count}");
        foreach (var kvp in dict)
        {
            Console.WriteLine($"    [{kvp.Key}] = {kvp.Value}");
        }

        // ToHashSet()
        Console.WriteLine("\nToHashSet():");
        var uniqueNumbers = new[] { 1, 2, 2, 3, 3, 3, 4 }.ToHashSet();
        Console.WriteLine($"  Type: {uniqueNumbers.GetType().Name}, Count: {uniqueNumbers.Count}, Items: {string.Join(", ", uniqueNumbers)}");

        // ToDictionary with grouping
        Console.WriteLine("\nToDictionary with grouping:");
        var people2 = new[] 
        { 
            (Name: "Alice", Department: "HR"),
            (Name: "Bob", Department: "IT"),
            (Name: "Charlie", Department: "HR"),
        };
        var grouped = people2
            .GroupBy(p => p.Department)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Name).ToList());
        foreach (var dept in grouped)
        {
            Console.WriteLine($"  {dept.Key}: {string.Join(", ", dept.Value)}");
        }
    }
}
