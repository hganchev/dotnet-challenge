using Generics.GenericClasses;
using Generics.GenericMethods;
using Generics.TypeConstraints;
using Generics.Variance;
using Generics.Examples;

Console.WriteLine("=== Week 1 — Generics ===");

// --- Generic Classes ---
Console.WriteLine("\n--- Generic Classes ---");

// Foo<T>: basic generic wrapper
var fooInt = new Foo<int>(99);
var fooStr = new Foo<string>("generic");
Console.WriteLine($"Foo<int>: {fooInt.GetValue()}, Foo<string>: {fooStr.GetValue()}");

// GenericPair<TFirst, TSecond>
var pair = new GenericPair<string, int>("answer", 42);
Console.WriteLine($"Pair: {pair}");
Console.WriteLine($"Swapped: {pair.Swap()}");

// GenericRepository<T>
var repo = new GenericRepository<Product>();
repo.Add(new Product { Id = 1, Name = "Widget",  Price = 9.99m });
repo.Add(new Product { Id = 2, Name = "Gadget",  Price = 24.99m });
repo.Add(new Product { Id = 3, Name = "Doohickey", Price = 4.49m });
Console.WriteLine($"Repository count: {repo.Count}");
Console.WriteLine($"Find cheap: {repo.Find(p => p.Price < 10)}");

// --- Generic Methods ---
Console.WriteLine("\n--- Generic Methods ---");

var gm = new GenericMethod();
Console.WriteLine($"Identity<string>: {gm.Identity("hello")}");
Console.WriteLine($"FirstElement<int>: {gm.FirstElement(new[] { 10, 20, 30 })}");

// UtilityMethods — type inference in action
int x = 5, y = 3;
UtilityMethods.Swap(ref x, ref y);
Console.WriteLine($"After Swap: x={x}, y={y}");
Console.WriteLine($"Min(7, 2): {UtilityMethods.Min(7, 2)}");
Console.WriteLine($"Clamp(15, 0, 10): {UtilityMethods.Clamp(15, 0, 10)}");
var mapped = UtilityMethods.Map([1, 2, 3, 4], n => n * n);
Console.WriteLine($"Map(n => n*n): [{string.Join(", ", mapped)}]");

// --- Type Constraints ---
ClassConstraintExamples.Run();
StructConstraintExamples.Run();
NewConstraintExamples.Run();
InterfaceConstraintExamples.Run();
MultipleConstraintsExamples.Run();

// --- Variance ---
CovariantExamples.Run();
ContravariantExamples.Run();

// --- Examples ---
GenericCollections.Run();
GenericDelegates.Run();
GenericInterfaces.Run();

