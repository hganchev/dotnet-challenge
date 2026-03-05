namespace ValueVsReference.EqualitySemantics;

// Reference type without overridden Equals — uses default reference equality.
public class Box
{
    public int Size { get; }
    public Box(int size) { Size = size; }
}
