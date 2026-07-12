using System.IO;
using OOP.Exceptions;

namespace OOP.Examples;

/// <summary>
/// Demonstrates exception fundamentals: catching specific types (not bare
/// <see cref="Exception"/>), custom exceptions with contextual data, exception
/// filters (<c>when</c>), correct rethrow (<c>throw;</c> vs <c>throw ex;</c>),
/// and using exceptions only for truly exceptional conditions.
/// </summary>
public static class ExceptionsDemo
{
    private static readonly Dictionary<int, decimal> Orders = new()
    {
        [1] = 42.50m,
        [2] = 17.00m,
    };

    public static void Run()
    {
        Console.WriteLine("\n=== Exceptions ===");

        // --- Catch a specific, meaningful exception type ---
        Console.WriteLine("\n--- Catching a specific custom exception ---");
        try
        {
            GetOrderTotal(orderId: 99);
        }
        catch (OrderNotFoundException ex)
        {
            Console.WriteLine($"  Handled: {ex.Message} (OrderId={ex.OrderId})");
        }

        // --- Custom exception carrying structured context, not string-parsed ---
        Console.WriteLine("\n--- Custom exception with structured context ---");
        try
        {
            ReserveStock(sku: "WIDGET-1", requested: 10, available: 3);
        }
        catch (InsufficientStockException ex)
        {
            Console.WriteLine($"  Handled: {ex.Message}");
            Console.WriteLine($"  Sku={ex.Sku}, Requested={ex.Requested}, Available={ex.Available}");
        }

        // --- Exception filters: inspect without a catch-then-rethrow ---
        Console.WriteLine("\n--- Exception filter (catch ... when ...) ---");
        try
        {
            ReadConfigFile("missing-config.json");
        }
        catch (FileNotFoundException ex) when (ex.FileName?.EndsWith(".json") == true)
        {
            Console.WriteLine($"  Filtered catch: JSON config file missing ('{ex.FileName}') — falling back to defaults.");
        }

        // --- throw; preserves the original stack trace, throw ex; does not ---
        Console.WriteLine("\n--- Rethrow: throw; vs throw ex; ---");
        Console.WriteLine("  " + DescribeRethrowBehaviour());

        Console.WriteLine("\n  Exceptions are for exceptional conditions only — validation");
        Console.WriteLine("  failures should return a result/bool, not throw (see Week5 Validation).");
    }

    private static decimal GetOrderTotal(int orderId)
    {
        // Catch only what we can meaningfully handle — a missing dictionary
        // entry is an expected, specific failure, not a bare Exception.
        if (!Orders.TryGetValue(orderId, out var total))
        {
            throw new OrderNotFoundException(orderId);
        }

        return total;
    }

    private static void ReserveStock(string sku, int requested, int available)
    {
        if (requested > available)
        {
            throw new InsufficientStockException(sku, requested, available);
        }
    }

    private static void ReadConfigFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Configuration file not found.", path);
        }
    }

    private static string DescribeRethrowBehaviour()
    {
        try
        {
            try
            {
                throw new InvalidOperationException("Original failure deep in the call stack.");
            }
            catch (InvalidOperationException)
            {
                throw; // preserves the original stack trace
            }
        }
        catch (InvalidOperationException ex)
        {
            // ex.StackTrace still points at the original throw site above,
            // not at this catch block — that's what `throw;` guarantees.
            return $"Caught via 'throw;' — stack trace still originates from the inner throw ({ex.Message}).";
        }
    }
}
