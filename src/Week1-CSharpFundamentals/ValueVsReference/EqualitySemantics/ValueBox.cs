namespace ValueVsReference.EqualitySemantics;

// Reference type with overridden Equals — value equality for a class.
public class ValueBox : IEquatable<ValueBox>
{
    public int Size { get; }
    public ValueBox(int size) { Size = size; }

    public bool Equals(ValueBox? other) => other is not null && Size == other.Size;
    public override bool Equals(object? obj) => Equals(obj as ValueBox);
    public override int GetHashCode() => HashCode.Combine(Size);
    public static bool operator ==(ValueBox? a, ValueBox? b) =>
        a is null ? b is null : a.Equals(b);
    public static bool operator !=(ValueBox? a, ValueBox? b) => !(a == b);
}
