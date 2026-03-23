namespace ClrInternals.Concepts;

/// <summary>
/// A small, immutable value type (8 bytes on 64-bit).
/// Small structs are efficient to copy by value — no need to pass by ref.
/// They live on the stack (locals/parameters) or inline in their container.
/// </summary>
public readonly struct SmallStruct
{
    public int X { get; init; }
    public int Y { get; init; }

    public SmallStruct(int x, int y) { X = x; Y = y; }

    public int Sum => X + Y;
    public double Distance => Math.Sqrt((double)X * X + (double)Y * Y);

    public override string ToString() => $"Point({X}, {Y})";

    // Value types use structural equality by default via override
    public override bool Equals(object? obj) => obj is SmallStruct other && X == other.X && Y == other.Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}
