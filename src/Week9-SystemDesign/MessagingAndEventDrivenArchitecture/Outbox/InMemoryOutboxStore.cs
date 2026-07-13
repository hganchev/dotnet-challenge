namespace MessagingAndEventDrivenArchitecture.Outbox;

/// <summary>Stores outbox messages alongside business data. In a real system this would be a table
/// in the same database as the business entities, written in the same transaction/SaveChanges call.</summary>
public class InMemoryOutboxStore
{
    private readonly List<OutboxMessage> _messages = [];

    public void Add(OutboxMessage message) => _messages.Add(message);

    public IReadOnlyList<OutboxMessage> GetPending() => _messages.Where(m => !m.Dispatched).ToList();

    public void MarkDispatched(OutboxMessage message) => message.MarkDispatched();
}
