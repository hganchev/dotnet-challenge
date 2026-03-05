namespace Generics.GenericClasses;

// A generic class with two type parameters
public class GenericPair<TFirst, TSecond>
{
    public TFirst First { get; }
    public TSecond Second { get; }

    public GenericPair(TFirst first, TSecond second)
    {
        First = first;
        Second = second;
    }

    public void Deconstruct(out TFirst first, out TSecond second)
    {
        first = First;
        second = Second;
    }

    public GenericPair<TSecond, TFirst> Swap() => new(Second, First);

    public override string ToString() => $"({First}, {Second})";
}
