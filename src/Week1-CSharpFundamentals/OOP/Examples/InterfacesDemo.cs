using OOP.Interfaces;

namespace OOP.Examples;

public static class InterfacesDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Interfaces, Default Methods & Explicit Implementation ===");

        var circle = new Circle(5);
        var rect   = new Rectangle(4, 6);

        // Default interface method (C# 8+): accessible via interface reference only
        IShape circleShape = circle;
        IShape rectShape   = rect;
        Console.WriteLine(circleShape.Describe());
        Console.WriteLine(rectShape.Describe());

        // Circle exposes Draw() publicly
        circle.Draw();

        // Rectangle uses explicit implementation: Draw() only via IDrawable reference
        // rect.Draw();          // compile error — not accessible on Rectangle directly
        IDrawable drawable = rect;
        drawable.Draw();
        rect.Render();            // non-interface public method is fine

        // Polymorphism through IShape
        Console.WriteLine("\n--- IShape polymorphism ---");
        IShape[] shapes = [circle, rect, new Circle(2), new Rectangle(10, 3)];
        double total = shapes.Sum(s => s.Area());
        Console.WriteLine($"Total area of {shapes.Length} shapes: {total:F2}");

        // IEquatable<T> and IComparable<T>
        Console.WriteLine("\n--- IEquatable<T> & IComparable<T> ---");
        var employees = new List<Employee>
        {
            new(3, "Carol", 95_000m),
            new(1, "Alice", 80_000m),
            new(2, "Bob",   80_000m),
        };

        employees.Sort();  // uses IComparable<Employee>.CompareTo — sorts by Salary
        Console.WriteLine("Sorted by salary:");
        foreach (var e in employees) Console.WriteLine($"  {e}");

        var e1 = new Employee(1, "Alice",           80_000m);
        var e2 = new Employee(1, "Alice Duplicate", 99_000m);  // same Id, different name/salary
        Console.WriteLine($"\ne1.Equals(e2) [same Id]: {e1.Equals(e2)}");
        Console.WriteLine($"e1 == e2:                 {e1 == e2}");
    }
}
