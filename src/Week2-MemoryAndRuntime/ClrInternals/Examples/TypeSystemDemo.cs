using System.Runtime.CompilerServices;
using ClrInternals.Concepts;

namespace ClrInternals.Examples;

/// <summary>
/// Explores the CLR type system: object layout, value vs reference types,
/// <c>typeof</c> vs <c>GetType()</c>, <see cref="RuntimeTypeHandle"/>,
/// and memory sizes via <see cref="Unsafe.SizeOf{T}"/>.
/// </summary>
public static class TypeSystemDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== CLR Type System ===");

        // 1. Object layout: every reference-type object has a 16-byte header
        //    (on 64-bit: 8-byte sync block index slot + 8-byte method table pointer)
        //    Value types have NO header — they are just their fields.
        Console.WriteLine("\n--- Object layout ---");
        Console.WriteLine("  Reference type object layout (64-bit):");
        Console.WriteLine("    [8 bytes] Sync block index (for lock/hash/thin-lock)");
        Console.WriteLine("    [8 bytes] Method Table pointer (type identity + vtable)");
        Console.WriteLine("    [n bytes] Fields");
        Console.WriteLine("  Value type layout: just the fields — no header overhead.");

        // 2. Unsafe.SizeOf<T>: size of T's value representation
        //    For ref types: pointer size (8 bytes on 64-bit)
        //    For value types: struct size
        Console.WriteLine("\n--- Unsafe.SizeOf<T> ---");
        PrintSize<byte>("byte");
        PrintSize<int>("int");
        PrintSize<long>("long");
        PrintSize<SmallStruct>("SmallStruct");   // 2× int = 8
        PrintSize<LargeStruct>("LargeStruct");   // 16× long = 128
        PrintSize<string>("string");             // pointer = 8 (not the string content!)
        PrintSize<object>("object");             // pointer = 8
        PrintSize<List<int>>("List<int>");       // pointer = 8 (reference type)

        // 3. typeof(T) vs obj.GetType() — compile-time vs runtime type
        Console.WriteLine("\n--- typeof vs GetType ---");
        object obj1 = new SmallStruct(1, 2);    // boxed — actual runtime type is SmallStruct
        object obj2 = "hello";

        Console.WriteLine($"  typeof(object)          = {typeof(object)}");
        Console.WriteLine($"  obj1.GetType()          = {obj1.GetType()}  (SmallStruct, not object)");
        Console.WriteLine($"  obj2.GetType()          = {obj2.GetType()}");
        Console.WriteLine($"  typeof(SmallStruct)     = {typeof(SmallStruct)}");
        Console.WriteLine($"  Same?                   = {obj1.GetType() == typeof(SmallStruct)}");

        // 4. RuntimeTypeHandle — lightweight token representing a type at runtime
        Console.WriteLine("\n--- RuntimeTypeHandle ---");
        RuntimeTypeHandle handle = typeof(SmallStruct).TypeHandle;
        Type fromHandle = Type.GetTypeFromHandle(handle)!;
        Console.WriteLine($"  TypeHandle value: 0x{handle.Value:X}");
        Console.WriteLine($"  Type from handle: {fromHandle.FullName}");

        // 5. Type hierarchy traversal
        Console.WriteLine("\n--- Base type chain ---");
        PrintBaseChain(typeof(SmallStruct));
        Console.WriteLine();
        PrintBaseChain(typeof(List<int>));

        // 6. Value type vs reference type key facts
        Console.WriteLine("\n--- Value type vs reference type ---");
        Console.WriteLine("  Value types: struct, enum, primitive numeric types");
        Console.WriteLine("    • Allocated inline (stack, field, array element)");
        Console.WriteLine("    • Copied on assignment");
        Console.WriteLine("    • No object header overhead");
        Console.WriteLine("  Reference types: class, interface, delegate, record class");
        Console.WriteLine("    • Allocated on the heap");
        Console.WriteLine("    • Variable holds a pointer (8 bytes)");
        Console.WriteLine("    • 16-byte header per object");
    }

    private static void PrintSize<T>(string label) =>
        Console.WriteLine($"  sizeof({label,-18}) = {Unsafe.SizeOf<T>(),3} bytes  ({(typeof(T).IsValueType ? "value type" : "reference — pointer size")})");

    private static void PrintBaseChain(Type type)
    {
        Console.Write($"  {type.Name}");
        Type? current = type.BaseType;
        while (current is not null)
        {
            Console.Write($" → {current.Name}");
            current = current.BaseType;
        }
        Console.WriteLine();
    }
}
