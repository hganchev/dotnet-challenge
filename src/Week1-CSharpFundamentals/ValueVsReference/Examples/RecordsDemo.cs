using ValueVsReference.Records;

namespace ValueVsReference.Examples;

// record class (C# 9+): reference type with compiler-generated value-based equality,
//                        ToString, and non-destructive mutation via 'with'.
// record struct (C# 10+): value type with the same compiler-generated members.
// Positional records: concise syntax; compiler generates constructor + deconstructor.

public static class RecordsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Records ===");

        // --- Value-based equality for record class ---
        var p1 = new Point2D(1.0, 2.0);
        var p2 = new Point2D(1.0, 2.0);
        var p3 = new Point2D(3.0, 4.0);

        Console.WriteLine($"p1 == p2 (same values):  {p1 == p2}");   // True
        Console.WriteLine($"p1 == p3 (diff values):  {p1 == p3}");   // False
        Console.WriteLine($"ReferenceEquals(p1,p2):  {ReferenceEquals(p1, p2)}"); // False

        // --- Compiler-generated ToString ---
        Console.WriteLine($"p1.ToString(): {p1}");  // Point2D { X = 1, Y = 2 }

        // --- Non-destructive mutation: 'with' expression ---
        var p4 = p1 with { Y = 99.0 };
        Console.WriteLine($"p1 with Y=99: {p4}");   // new object, p1 unchanged
        Console.WriteLine($"p1 unchanged: {p1}");

        // --- Deconstruction ---
        var (x, y) = p1;
        Console.WriteLine($"Deconstructed: x={x}, y={y}");

        // --- Nominal record with init-only properties ---
        var person = new Person { FirstName = "Alice", LastName = "Smith", Age = 30 };
        var older  = person with { Age = 31 };
        Console.WriteLine($"person: {person}");
        Console.WriteLine($"older:  {older}");
        Console.WriteLine($"person == older: {person == older}");  // False — Age differs

        // --- Record inheritance: positional params chained to base ---
        var dog = new Dog("Rex", "Labrador");
        Console.WriteLine($"Dog: {dog}");  // Dog { Name = Rex, Breed = Labrador }

        Animal animal = dog;               // polymorphism
        Console.WriteLine($"As Animal: {animal.Name}");

        // --- record struct: value type ---
        var t1 = new Temperature(100.0);
        var t2 = new Temperature(100.0);
        Console.WriteLine($"record struct t1 == t2: {t1 == t2}");      // True (value equality)
        Console.WriteLine($"100°C in Fahrenheit: {t1.Fahrenheit}°F");

        var t3 = t1 with { Celsius = 0.0 };
        Console.WriteLine($"t3 (0°C): {t3.Fahrenheit}°F");
    }
}
