namespace ResilienceAndRateLimiting.Idempotency;

/// <summary>Stores responses keyed by a client-supplied idempotency key so a duplicate request can
/// be answered without repeating the underlying (non-idempotent) operation.</summary>
public class IdempotencyStore
{
    private readonly Dictionary<string, string> _responsesByKey = new();

    public bool TryGetExisting(string idempotencyKey, out string existingResponse) =>
        _responsesByKey.TryGetValue(idempotencyKey, out existingResponse!);

    public void Save(string idempotencyKey, string response) => _responsesByKey[idempotencyKey] = response;
}
