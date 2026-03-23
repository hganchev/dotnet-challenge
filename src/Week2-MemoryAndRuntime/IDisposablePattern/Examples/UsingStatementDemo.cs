using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates the two syntactic forms of the using statement
/// and proves that Dispose() is called even when an exception is thrown.
/// </summary>
public static class UsingStatementDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== using Statement vs using Declaration ===");

        // 1. Classic using statement — braces define the scope explicitly
        Console.WriteLine("\n--- Classic: using (var x = ...) { } ---");
        using (var holder = new ManagedResourceHolder("ClassicUsing"))
        {
            holder.Write("data inside classic using block");
            Console.WriteLine("  Inside using block.");
        } // Dispose() called here
        Console.WriteLine("  After using block — Dispose() already ran.");

        // 2. Using declaration — scope is the enclosing block
        Console.WriteLine("\n--- Modern: using var x = ... ---");
        UsingDeclarationScope();

        // 3. Dispose is called even when an exception is thrown
        Console.WriteLine("\n--- Dispose() called even on exception ---");
        DisposeOnException();

        // 4. Nested using — inner disposed before outer
        Console.WriteLine("\n--- Nested using: inner disposed before outer ---");
        using var outer = new ManagedResourceHolder("Outer");
        using var inner = new ManagedResourceHolder("Inner");
        inner.Write("inner data");
        outer.Write("outer data");
        Console.WriteLine("  Both created. Scope ends — Inner disposes first, then Outer.");
        // Dispose order: inner first, outer second (reverse declaration order)
    }

    private static void UsingDeclarationScope()
    {
        using var holder = new ManagedResourceHolder("ModernUsing");
        holder.Write("data inside method using declaration");
        Console.WriteLine("  Inside method with using declaration.");
    } // Dispose() called here at end of method
    // Note: Console.WriteLine after this would be OUTSIDE the lifetime

    private static void DisposeOnException()
    {
        try
        {
            using var holder = new ManagedResourceHolder("ExceptionHolder");
            holder.Write("before exception");
            Console.WriteLine("  About to throw...");
            throw new InvalidOperationException("Simulated error.");
        }
        catch (InvalidOperationException ex)
        {
            // Dispose() was called before we reached this catch block
            Console.WriteLine($"  Caught: {ex.Message}");
            Console.WriteLine("  Dispose() was still called before the exception propagated.");
        }
    }
}
