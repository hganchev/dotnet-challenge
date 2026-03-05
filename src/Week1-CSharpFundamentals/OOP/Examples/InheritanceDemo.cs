using OOP.Inheritance;

namespace OOP.Examples;

public static class InheritanceDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Inheritance & Abstract Classes ===");

        // Concrete subclasses of abstract Animal
        Dog            rex     = new("Rex",     3, "German Shepherd");
        Cat            whiskers = new("Whiskers", 5);
        GoldenRetriever buddy  = new("Buddy",   2);  // sealed — cannot subclass further

        Console.WriteLine(rex.Describe());
        Console.WriteLine($"  says: {rex.Speak()}");

        Console.WriteLine(whiskers.Describe());
        Console.WriteLine($"  says: {whiskers.Speak()}");

        // GoldenRetriever's Describe() chains: GoldenRetriever → Dog → Animal
        Console.WriteLine(buddy.Describe());
        Console.WriteLine($"  says: {buddy.Speak()}");

        // Polymorphic array: Animal[] can hold any concrete subtype
        Console.WriteLine("\n--- Polymorphic Animal[] ---");
        Animal[] animals = [rex, whiskers, buddy];
        foreach (var a in animals)
            Console.WriteLine($"  {a.GetType().Name,-18} {a.Name,-10} → {a.Speak()}");

        // Animal is abstract: cannot be instantiated
        // Animal a = new Animal("x", 0);  // compile error
    }
}
