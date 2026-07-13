namespace ResilienceAndRateLimiting.Idempotency;

/// <summary>Simulates a "create order" style operation made safe for client retries via an
/// idempotency key: the same key always returns the original response instead of creating a
/// duplicate order.</summary>
public class OrderCreationService
{
    private readonly IdempotencyStore _store;
    private int _ordersCreated;

    public OrderCreationService(IdempotencyStore store) => _store = store;

    public string CreateOrder(string idempotencyKey, string orderPayload)
    {
        if (_store.TryGetExisting(idempotencyKey, out var existingResponse))
        {
            Console.WriteLine($"  [OrderCreationService] duplicate request (key={idempotencyKey}) — returning cached response, no new order created");
            return existingResponse;
        }

        _ordersCreated++;
        var response = $"ORDER-{_ordersCreated:D4}";
        _store.Save(idempotencyKey, response);
        Console.WriteLine($"  [OrderCreationService] created {response} for payload '{orderPayload}' (key={idempotencyKey})");
        return response;
    }
}
