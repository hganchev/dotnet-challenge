namespace OOP.Interfaces;

// Implements both IShape and IDrawable with standard (public) implementations.
public class Circle : IShape, IDrawable
{
    public double Radius { get; }

    public Circle(double radius) => Radius = radius;

    public double Area()      => Math.PI * Radius * Radius;
    public double Perimeter() => 2 * Math.PI * Radius;

    // Public Draw(): callable via both Circle and IDrawable references
    public void Draw() => Console.WriteLine($"  ◯  Circle (r={Radius})");
}
