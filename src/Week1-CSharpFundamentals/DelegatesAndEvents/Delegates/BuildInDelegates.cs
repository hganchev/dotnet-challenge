namespace DelegatesAndEvents.Delegates;

public class BuildInDelegates
{
    public void Run()
    {
        // Using Action delegate (takes parameters and returns void)
        Action<string> printMessage = message => Console.WriteLine(message);
        printMessage("Hello, Action delegate!"); // Output: Hello, Action delegate!

        // Using Func delegate (takes parameters and returns a value)
        Func<int, int, int> add = (a, b) => a + b;
        int result = add(5, 3);
        Console.WriteLine($"The sum is: {result}"); // Output: The sum is: 8

        // Using Predicate delegate (takes a parameter and returns a bool)
        Predicate<int> isEven = number => number % 2 == 0;
        bool checkEven = isEven(4);
        Console.WriteLine($"Is the number even? {checkEven}"); // Output: Is the number even? True
    }
}