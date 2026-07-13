namespace MessagingAndEventDrivenArchitecture.Outbox;

/// <summary>Separately drains pending outbox messages and publishes them — decoupled from the
/// business transaction that created them.</summary>
public class OutboxDispatcher
{
    private readonly InMemoryOutboxStore _store;

    public OutboxDispatcher(InMemoryOutboxStore store) => _store = store;

    public async Task DispatchPendingAsync()
    {
        foreach (var message in _store.GetPending())
        {
            await Task.Delay(50); // simulate publishing to a broker
            Console.WriteLine($"  [OutboxDispatcher] published '{message.EventType}' for {message.Payload}");
            _store.MarkDispatched(message);
        }
    }
}
