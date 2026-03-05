// Only the Classic namespace is imported here — no collision with Modern.
using ExtensionMethods.Classic;

/// <summary>Demonstrates the classic C# ≤ 13 / .NET 9 extension method style.</summary>
public static class ClassicDemo
{
    private const string Sample   = "Hello, World!";
    private const string Sentence = "the quick brown fox";

    public static void Run()
    {
        Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  .NET 9 / C# ≤ 13 — Classic extension methods           │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");

        // All extensions look like instance methods at the call site …
        Console.WriteLine($"  \"racecar\".IsPalindrome()  → {"racecar".IsPalindrome()}");
        Console.WriteLine($"  \"hello\".IsPalindrome()    → {"hello".IsPalindrome()}");

        // … but properties are impossible — WordCount must use ()
        Console.WriteLine($"  \"{Sentence}\".WordCount()   → {Sentence.WordCount()}   ← must use ()");
        Console.WriteLine($"  \"{Sample}\".Truncate(8)    → {Sample.Truncate(8)}");
        Console.WriteLine($"  \"{Sentence}\".ToTitleCase() → {Sentence.ToTitleCase()}");

        Console.WriteLine();
        Console.WriteLine("  Limitations in C# ≤ 13:");
        Console.WriteLine("    ✘ No extension properties   — s.WordCount()  not  s.WordCount");
        Console.WriteLine("    ✘ No extension static members — can't add  string.Separator");
        Console.WriteLine("    ✘ No extension operators    — can't write  \"ha\" * 3");
        Console.WriteLine("    ✘ Extensions for one type scattered across unrelated static classes");
        Console.WriteLine();
    }
}
