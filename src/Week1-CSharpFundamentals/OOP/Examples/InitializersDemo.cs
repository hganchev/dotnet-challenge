using OOP.Encapsulation;
using OOP.Inheritance;

namespace OOP.Examples;

public static class InitializersDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Object & Collection Initializers ===");

        // Object initializer: sets init-only and regular properties after the constructor call
        var person = new PersonInfo
        {
            FirstName = "Charlie",
            LastName  = "Brown",
            Age       = 30,
            Email     = "charlie@example.com"
        };
        Console.WriteLine($"Object initializer: {person}");

        // BankAccount with Deposit called immediately after construction
        var account = new BankAccount("Dave", "ACC-200", 500m);
        Console.WriteLine($"Constructor only:   {account}");

        // Collection initializer: each element calls Add() under the hood
        Console.WriteLine("\n--- Collection initializer ---");
        var animals = new List<Animal>
        {
            new Dog("Rex",    5, "Labrador"),
            new Cat("Luna",   3),
            new GoldenRetriever("Max", 1)
        };
        foreach (var a in animals)
            Console.WriteLine($"  {a}");

        // Collection expression (C# 12): concise syntax for arrays / List<T>
        Console.WriteLine("\n--- Collection expression (C# 12) ---");
        List<string> names = ["Alice", "Bob", "Charlie"];
        Console.WriteLine($"  [{string.Join(", ", names)}]");

        // Target-typed new: compiler infers the type from the variable declaration
        Console.WriteLine("\n--- Target-typed new ---");
        Dictionary<string, int> scores = new()
        {
            ["Alice"]   = 95,
            ["Bob"]     = 87,
            ["Charlie"] = 92
        };
        foreach (var (name, score) in scores)
            Console.WriteLine($"  {name}: {score}");
    }
}
