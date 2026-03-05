#if NET10_0_OR_GREATER
// Only the Modern namespace is imported here — no collision with Classic.
using ExtensionMethods.Modern;

/// <summary>Demonstrates the C# 14 / .NET 10 extension member block syntax.</summary>
public static class ModernDemo
{
    private const string Sample   = "Hello, World!";
    private const string Sentence = "the quick brown fox";

    public static void Run()
    {
        Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  .NET 10 / C# 14 — New extension member blocks          │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        // Extension *properties* — accessed without parentheses
        Console.WriteLine($"  \"\".IsEmpty              → {"".IsEmpty}   ← property, no ()");
        Console.WriteLine($"  \"hi\".IsEmpty            → {"hi".IsEmpty}");
        Console.WriteLine($"  \"racecar\".IsPalindrome  → {"racecar".IsPalindrome}   ← property, no ()");
        Console.WriteLine($"  \"hello\".IsPalindrome    → {"hello".IsPalindrome}");
        Console.WriteLine($"  \"{Sentence}\".WordCount   → {Sentence.WordCount}       ← property, no ()");

        // Extension methods — same behaviour, but co-located with the properties above
        Console.WriteLine($"  \"{Sample}\".Truncate(8)   → {Sample.Truncate(8)}");
        Console.WriteLine($"  \"{Sentence}\".ToTitleCase() → {Sentence.ToTitleCase()}");

        // Extension *static* members — new in C# 14
        Console.WriteLine();
        Console.WriteLine($"  string.Separator        → {string.Separator}");
        Console.WriteLine($"  string.Repeat(\"ab\", 4)  → {string.Repeat("ab", 4)}");

        // Extension *operator* — new in C# 14
        Console.WriteLine();
        Console.WriteLine($"  \"ha\" * 3               → {"ha" * 3}");
        Console.WriteLine($"  \"-\" * 20               → {"-" * 20}");

        Console.WriteLine();
        Console.WriteLine("  New capabilities in C# 14:");
        Console.WriteLine("    ✔ Extension properties    — .WordCount, .IsEmpty, .IsPalindrome");
        Console.WriteLine("    ✔ Extension static members — string.Separator, string.Repeat(...)");
        Console.WriteLine("    ✔ Extension operators      — \"ha\" * 3");
        Console.WriteLine("    ✔ All extensions for a type grouped in one extension(...) block");
        Console.WriteLine();
    }
}
#endif
