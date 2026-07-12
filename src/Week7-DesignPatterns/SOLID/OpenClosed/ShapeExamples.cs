namespace SOLID.OpenClosed;

/// <summary>Violates OCP: adding a new shape means editing this class's switch statement.</summary>
public sealed class AreaCalculatorBad
{
    public double CalculateArea(object shape) => shape switch
    {
        Rectangle r => r.Width * r.Height,
        Circle c => Math.PI * c.Radius * c.Radius,
        // Adding Triangle here would require modifying this existing, already-shipped method.
        _ => throw new NotSupportedException($"Unknown shape: {shape.GetType().Name}")
    };

    public sealed class Rectangle
    {
        public double Width { get; init; }
        public double Height { get; init; }
    }

    public sealed class Circle
    {
        public double Radius { get; init; }
    }
}

/// <summary>Open for extension (new shapes implement <see cref="IShape"/>), closed for modification.</summary>
public interface IShape
{
    double CalculateArea();
}

public sealed class Rectangle(double width, double height) : IShape
{
    public double Width { get; } = width;
    public double Height { get; } = height;

    public double CalculateArea() => Width * Height;
}

public sealed class Circle(double radius) : IShape
{
    public double Radius { get; } = radius;

    public double CalculateArea() => Math.PI * Radius * Radius;
}

/// <summary>New shape added without touching <see cref="AreaCalculator"/>.</summary>
public sealed class Triangle(double baseLength, double height) : IShape
{
    public double BaseLength { get; } = baseLength;
    public double Height { get; } = height;

    public double CalculateArea() => 0.5 * BaseLength * Height;
}

public sealed class AreaCalculator
{
    public double CalculateTotalArea(IEnumerable<IShape> shapes) => shapes.Sum(s => s.CalculateArea());
}
