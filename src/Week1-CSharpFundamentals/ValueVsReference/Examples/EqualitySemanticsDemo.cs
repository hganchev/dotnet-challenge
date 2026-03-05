using ValueVsReference.EqualitySemantics;

namespace ValueVsReference.Examples;

// == (operator)      — by default: reference equality for classes, value equality for structs.
//                      Can be overloaded.
// Equals()           — virtual method; classes should override for value equality.
// ReferenceEquals()  — always compares object identity (heap address); cannot be overloaded.
// IEquatable<T>      — strongly-typed equality; avoids boxing; implement alongside Equals(object).

public static class EqualitySemanticsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Equality Semantics ===");

        // --- Reference type without overridden Equals (reference equality) ---
        var box1 = new Box(10);
        var box2 = new Box(10);
        var box3 = box1;

        Console.WriteLine($"box1 == box2 (same size, diff object): {box1 == box2}");       // False
        Console.WriteLine($"box1.Equals(box2):                      {box1.Equals(box2)}"); // False
        Console.WriteLine($"box1 == box3 (same reference):          {box1 == box3}");      // True
        Console.WriteLine($"ReferenceEquals(box1,box2):             {ReferenceEquals(box1, box2)}"); // False
        Console.WriteLine($"ReferenceEquals(box1,box3):             {ReferenceEquals(box1, box3)}"); // True

        // --- Reference type with overridden Equals (value equality) ---
        var vbox1 = new ValueBox(10);
        var vbox2 = new ValueBox(10);
        Console.WriteLine($"ValueBox vbox1 == vbox2: {vbox1 == vbox2}");    // True
        Console.WriteLine($"ReferenceEquals(vbox1,vbox2): {ReferenceEquals(vbox1, vbox2)}"); // False

        // --- Struct equality ---
        var m1 = new Money(9.99m, "USD");
        var m2 = new Money(9.99m, "USD");
        var m3 = new Money(5.00m, "USD");

        Console.WriteLine($"m1 == m2 (same value):     {m1 == m2}");  // True
        Console.WriteLine($"m1 == m3 (diff value):     {m1 == m3}");  // False
        Console.WriteLine($"m1.Equals(m2):             {m1.Equals(m2)}"); // True

        // --- string: reference type but == compares by value ---
        string s1 = new string("hello".ToCharArray());  // not interned
        string s2 = new string("hello".ToCharArray());
        Console.WriteLine($"string == :              {s1 == s2}");              // True (overloaded)
        Console.WriteLine($"string ReferenceEquals:  {ReferenceEquals(s1, s2)}"); // False
        Console.WriteLine($"string Equals:           {s1.Equals(s2)}");        // True

        // --- GetHashCode contract: equal objects must have equal hash codes ---
        Console.WriteLine($"m1.GetHashCode() == m2.GetHashCode(): {m1.GetHashCode() == m2.GetHashCode()}"); // True
    }
}
