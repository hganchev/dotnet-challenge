namespace ValueVsReference.Examples;

// Nullable value types: int? is shorthand for Nullable<int>
// They add a bool HasValue flag and wrap the underlying T value.
// Nullable reference types (C# 8+) are a compile-time annotation — no runtime wrapper.

public static class NullableTypesDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Nullable Types ===");

        // --- Nullable<T> / T? ---
        int? maybeAge = null;
        Console.WriteLine($"maybeAge HasValue: {maybeAge.HasValue}");

        maybeAge = 25;
        Console.WriteLine($"maybeAge HasValue: {maybeAge.HasValue}, Value: {maybeAge.GetValueOrDefault()}");

        // Accessing .Value when null throws InvalidOperationException
        int? noValue = null;
#pragma warning disable CS8629
        try { _ = noValue.Value; }
#pragma warning restore CS8629
        catch (InvalidOperationException ex) { Console.WriteLine($"Ex: {ex.Message}"); }

        // --- GetValueOrDefault ---
        Console.WriteLine($"noValue.GetValueOrDefault(): {noValue.GetValueOrDefault()}");   // 0
        Console.WriteLine($"noValue.GetValueOrDefault(99): {noValue.GetValueOrDefault(99)}"); // 99

        // --- Null-coalescing operator ?? ---
        int age = maybeAge ?? -1;
        Console.WriteLine($"maybeAge ?? -1 = {age}");  // 25

        int fallback = noValue ?? 100;
        Console.WriteLine($"noValue ?? 100 = {fallback}");  // 100

        // --- Null-coalescing assignment ??= ---
        int? cached = null;
        cached ??= ComputeExpensiveValue();
        Console.WriteLine($"cached after ??=: {cached}");

        // --- Null-conditional operator ?. with nullable ---
        string? text = null;
        int? length = text?.Length;    // null — no NullReferenceException
        Console.WriteLine($"null?.Length = {length?.ToString() ?? "null"}");

        text = "hello";
        length = text?.Length;
        Console.WriteLine($"\"hello\"?.Length = {length}");

        // --- Null patterns ---
        PrintIfNotNull(null);
        PrintIfNotNull(42);

        // --- Lifting: arithmetic on nullable types ---
        int? a = 5;
        int? b = null;
        int? sum = a + b;  // null — any null operand yields null
        Console.WriteLine($"5 + null = {sum?.ToString() ?? "null"}");

        int? product = a * 3;
        Console.WriteLine($"5 * 3 (nullable) = {product}");
    }

    private static int ComputeExpensiveValue() => 42;

    private static void PrintIfNotNull(int? value)
    {
        if (value is null)
            Console.WriteLine("Value is null");
        else
            Console.WriteLine($"Value is {value}");
    }
}
