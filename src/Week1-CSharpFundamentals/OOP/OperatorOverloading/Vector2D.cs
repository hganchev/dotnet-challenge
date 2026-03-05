namespace OOP.OperatorOverloading;

// Immutable 2-D vector — a natural fit for operator overloading.
public sealed class Vector2D
{
    public double X { get; }
    public double Y { get; }

    public Vector2D(double x, double y) { X = x; Y = y; }

    // Derived property
    public double Magnitude => Math.Sqrt(X * X + Y * Y);

    // Arithmetic operators
    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2D operator *(Vector2D v, double s)   => new(v.X * s, v.Y * s);
    public static Vector2D operator *(double s, Vector2D v)   => v * s;   // commutativity

    // Equality operators — must override Equals and GetHashCode to keep them consistent
    public static bool operator ==(Vector2D a, Vector2D b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2D a, Vector2D b) => !(a == b);

    public override bool Equals(object? obj) => obj is Vector2D other && this == other;
    public override int  GetHashCode()        => HashCode.Combine(X, Y);

    // Implicit conversion: tuple → Vector2D (no cast needed, direction is obvious)
    public static implicit operator Vector2D((double x, double y) t) => new(t.x, t.y);

    // Explicit conversion: Vector2D → double[] (requires a cast, non-obvious loss of type info)
    public static explicit operator double[](Vector2D v) => [v.X, v.Y];

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
