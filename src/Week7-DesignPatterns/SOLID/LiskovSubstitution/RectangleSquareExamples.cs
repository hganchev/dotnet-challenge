namespace SOLID.LiskovSubstitution;

/// <summary>Classic LSP violation: <see cref="Square"/> strengthens <see cref="Rectangle"/>'s
/// invariants (width and height must stay equal), breaking code that treats it as a Rectangle.</summary>
public class Rectangle
{
    public virtual double Width { get; set; }
    public virtual double Height { get; set; }

    public double CalculateArea() => Width * Height;
}

/// <summary>Violates LSP: setting Width also mutates Height (and vice versa), which a caller
/// working against the Rectangle base type does not expect and cannot substitute safely.</summary>
public sealed class Square : Rectangle
{
    public override double Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }
    }

    public override double Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }
    }
}

/// <summary>Fix: model the shared behaviour via an abstraction instead of inheriting a type
/// whose invariants don't hold for both shapes. No shape "is-a" mutable Rectangle.</summary>
public interface IQuadrilateral
{
    double CalculateArea();
}

public sealed class FixedRectangle(double width, double height) : IQuadrilateral
{
    public double Width { get; } = width;
    public double Height { get; } = height;

    public double CalculateArea() => Width * Height;
}

public sealed class FixedSquare(double side) : IQuadrilateral
{
    public double Side { get; } = side;

    public double CalculateArea() => Side * Side;
}
