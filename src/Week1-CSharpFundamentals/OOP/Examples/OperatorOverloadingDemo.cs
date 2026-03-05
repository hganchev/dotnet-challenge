using OOP.OperatorOverloading;

namespace OOP.Examples;

public static class OperatorOverloadingDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Operator Overloading & Conversions ===");

        var v1 = new Vector2D(3, 4);
        var v2 = new Vector2D(1, 2);

        Console.WriteLine($"v1 = {v1},  |v1| = {v1.Magnitude:F2}");
        Console.WriteLine($"v2 = {v2}");
        Console.WriteLine($"v1 + v2  = {v1 + v2}");
        Console.WriteLine($"v1 - v2  = {v1 - v2}");
        Console.WriteLine($"v1 * 2   = {v1 * 2}");
        Console.WriteLine($"3  * v2  = {3 * v2}");

        // Equality operators (also overloads Equals / GetHashCode)
        var v3 = new Vector2D(3, 4);
        Console.WriteLine($"\nv1 == v3 (same values): {v1 == v3}");
        Console.WriteLine($"v1 == v2 (different):   {v1 == v2}");

        // Implicit conversion from tuple: no explicit cast needed
        Vector2D fromTuple = (5.0, 0.0);
        Console.WriteLine($"\nImplicit from (5.0, 0.0): {fromTuple}");

        // Explicit conversion to double[]: cast required
        double[] arr = (double[])v1;
        Console.WriteLine($"Explicit to double[]:     [{string.Join(", ", arr)}]");
    }
}
