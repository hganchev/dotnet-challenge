using ResilienceAndRateLimiting.Idempotency;

namespace ResilienceAndRateLimiting.Examples;

/// <summary>Demonstrates using an idempotency key to make a "create order" style operation safe to
/// retry: repeated requests with the same key return the original response instead of duplicating
/// the order.</summary>
public static class IdempotencyDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Idempotency Keys for Safe Retries ===");
        var store = new IdempotencyStore();
        var service = new OrderCreationService(store);

        var key = Guid.NewGuid().ToString();
        // Simulate a client retrying the same "create order" request 3 times
        // (e.g. after a network timeout) using the same idempotency key.
        service.CreateOrder(key, "2x Widget");
        service.CreateOrder(key, "2x Widget");
        service.CreateOrder(key, "2x Widget");

        // A different key represents a genuinely new order.
        service.CreateOrder(Guid.NewGuid().ToString(), "1x Gadget");
    }
}
