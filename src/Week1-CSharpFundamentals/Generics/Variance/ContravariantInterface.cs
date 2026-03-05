namespace Generics.Variance;

// Contravariance: in T — T can only appear in input positions (parameters)
// Allows IWriter<Base> to be assigned to IWriter<Derived>
public interface IWriter<in T>
{
    void Write(T value);
}

// Concrete implementation
public class ConsoleWriter<T> : IWriter<T>
{
    public void Write(T value) => Console.WriteLine($"  Writing: {value}");
}

public static class ContravariantExamples
{
    public static void Run()
    {
        Console.WriteLine("\n=== Contravariance: in T (IWriter<in T>) ===");

        // IComparer<object> is assignable to IComparer<string> — contravariance built in
        // A comparer that can compare any object can certainly compare strings
        IComparer<object> objComparer = Comparer<object>.Default;
        IComparer<string> strComparer = objComparer; // valid — in T is contravariant
        Console.WriteLine($"IComparer<object> → IComparer<string>: compare result = {strComparer.Compare("a", "b")}");

        // Custom contravariant interface
        IWriter<object> objWriter = new ConsoleWriter<object>();
        IWriter<string> strWriter = objWriter; // valid — in T is contravariant
        // A writer that can write any object can certainly write strings
        strWriter.Write("hello from contravariant writer");

        // Why contravariance is safe: we only WRITE (consume) T, never return it
        // Mnemonic: producers use out (covariant), consumers use in (contravariant)
        Console.WriteLine("Producer = out (covariant), Consumer = in (contravariant)");
    }
}
