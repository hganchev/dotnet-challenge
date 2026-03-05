namespace Generics.Examples;

public static class GenericDelegates
{
    public static void Run()
    {
        Console.WriteLine("\n=== Generic Delegates: Func<>, Action<>, Predicate<> ===");

        // Func<TResult> — no input, returns TResult
        Func<DateTime> now = () => DateTime.UtcNow;
        Console.WriteLine($"Func<DateTime>: {now():O}");

        // Func<T, TResult> — one input, one output
        Func<int, string> intToStr = n => $"Number({n})";
        Console.WriteLine($"Func<int,string>: {intToStr(42)}");

        // Func<T1, T2, TResult> — two inputs
        Func<double, double, double> hypotenuse = (a, b) => Math.Sqrt(a * a + b * b);
        Console.WriteLine($"Func<double,double,double> hypotenuse(3,4): {hypotenuse(3, 4)}");

        // Action<T> — one input, no return value
        Action<string> print = Console.WriteLine;
        print("  Action<string> called");

        // Action<T1, T2>
        Action<string, int> repeat = (s, n) =>
        {
            for (int i = 0; i < n; i++) Console.Write(s);
            Console.WriteLine();
        };
        Console.Write("  Action<string,int> repeat('* ', 4): ");
        repeat("* ", 4);

        // Predicate<T> — equivalent to Func<T, bool>
        Predicate<int> isEven = n => n % 2 == 0;
        Console.WriteLine($"Predicate<int> isEven(6): {isEven(6)}, isEven(7): {isEven(7)}");

        // Higher-order function — passing Func<> as a parameter
        var numbers = Enumerable.Range(1, 10).ToList();
        var evens = numbers.FindAll(isEven); // List<T>.FindAll takes Predicate<T>
        Console.WriteLine($"FindAll(isEven): [{string.Join(", ", evens)}]");

        // Composing delegates
        Func<int, int> doubleIt = x => x * 2;
        Func<int, int> addTen = x => x + 10;
        Func<int, int> composed = x => addTen(doubleIt(x));
        Console.WriteLine($"Composed (double then add 10) of 5: {composed(5)}");
    }
}
