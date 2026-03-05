namespace OOP.Interfaces;

// Uses explicit interface implementation for IDrawable.Draw():
// the method is only accessible when the reference is typed as IDrawable.
public class Rectangle : IShape, IDrawable
{
    public double Width  { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width  = width;
        Height = height;
    }

    public double Area()      => Width * Height;
    public double Perimeter() => 2 * (Width + Height);

    // Explicit implementation — forces callers to use an IDrawable reference
    void IDrawable.Draw() => Console.WriteLine($"  ▭  Rectangle ({Width}x{Height})");

    // Extra public method that isn't part of the IDrawable contract
    public void Render() => Console.WriteLine($"  Rendering Rectangle ({Width}x{Height})");
}
