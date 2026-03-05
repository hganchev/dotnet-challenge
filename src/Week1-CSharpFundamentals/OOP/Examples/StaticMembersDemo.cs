using OOP.StaticMembers;

namespace OOP.Examples;

public static class StaticMembersDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Static Members, Singleton & Extension Methods ===");

        // Singleton: every call returns the exact same instance
        var s1 = AppSettings.Instance;
        var s2 = AppSettings.Instance;
        Console.WriteLine($"Same instance: {ReferenceEquals(s1, s2)}");
        Console.WriteLine($"Before configure: {AppSettings.Instance}");
        AppSettings.Instance.Configure("MyApp", "2.1.0");
        Console.WriteLine($"After  configure: {AppSettings.Instance}");

        // Static utility class
        Console.WriteLine("\n--- MathUtils (static class) ---");
        Console.WriteLine($"IsPrime(17): {MathUtils.IsPrime(17)}");
        Console.WriteLine($"IsPrime(15): {MathUtils.IsPrime(15)}");
        Console.WriteLine($"Fibonacci(10): {MathUtils.Fibonacci(10)}");
        Console.WriteLine($"90° → radians: {MathUtils.DegreesToRadians(90):F4}");

        // Extension methods: called as if they were instance methods on string
        Console.WriteLine("\n--- StringExtensions (extension methods) ---");
        Console.WriteLine($"\"racecar\" IsPalindrome: {"racecar".IsPalindrome()}");
        Console.WriteLine($"\"hello\"   IsPalindrome: {"hello".IsPalindrome()}");
        Console.WriteLine($"Truncate(8): {"Hello, World!".Truncate(8)}");
        Console.WriteLine($"ToTitleCase: {"the quick brown fox".ToTitleCase()}");
    }
}
