using ValueVsReference.RefOutIn;

namespace ValueVsReference.Examples;

// ref  — pass by reference (read + write); caller must initialise before passing
// out  — pass by reference (write-only contract); callee must assign before returning
// in   — pass by reference (read-only); no copy, callee cannot modify

public static class RefOutInDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ref / out / in Parameters ===");

        // --- ref: read and write the caller's variable ---
        int value = 10;
        Console.WriteLine($"Before Increment: {value}");
        Increment(ref value);
        Console.WriteLine($"After  Increment: {value}");  // 11

        int x = 5, y = 3;
        Swap(ref x, ref y);
        Console.WriteLine($"After Swap: x={x}, y={y}");   // x=3, y=5

        // --- out: callee must assign; caller doesn't need to initialise ---
        bool parsed = TryParsePositive("42", out int result);
        Console.WriteLine($"TryParsePositive(\"42\"): success={parsed}, result={result}");

        bool parsedBad = TryParsePositive("-5", out int badResult);
        Console.WriteLine($"TryParsePositive(\"-5\"): success={parsedBad}, result={badResult}");

        // out variable can be inlined at the call site (C# 7+)
        if (int.TryParse("123", out int inlineResult))
            Console.WriteLine($"int.TryParse inline out: {inlineResult}");

        // Discard out parameter with _
        _ = int.TryParse("abc", out _);
        Console.WriteLine("Discarded out parameter with _");

        // --- in: read-only reference — avoids copying large structs ---
        var big = new LargeStruct(1, 2, 3, 4);
        Console.WriteLine($"Sum via in: {SumComponents(in big)}");

        // in parameter: compiler prevents mutation inside the method
        // (uncommenting the line in SumComponents would be a compile error)
    }

    private static void Increment(ref int n) => n++;

    private static void Swap(ref int a, ref int b) => (a, b) = (b, a);

    private static bool TryParsePositive(string s, out int number)
    {
        if (int.TryParse(s, out number) && number > 0)
            return true;

        number = 0;   // must assign out on all code paths
        return false;
    }

    private static int SumComponents(in LargeStruct s)
    {
        // s.A = 99; // compile error — 'in' is read-only
        return s.A + s.B + s.C + s.D;
    }
}
