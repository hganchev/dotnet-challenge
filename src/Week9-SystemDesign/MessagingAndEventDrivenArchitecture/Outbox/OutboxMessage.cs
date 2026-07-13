namespace MessagingAndEventDrivenArchitecture.Outbox;

/// <summary>A pending event queued alongside a business change, waiting to be published.</summary>
public class OutboxMessage
{
    public Guid Id { get; } = Guid.NewGuid();

    public string EventType { get; }

    public string Payload { get; }

    public bool Dispatched { get; private set; }

    public OutboxMessage(string eventType, string payload)
    {
        EventType = eventType;
        Payload = payload;
    }

    public void MarkDispatched() => Dispatched = true;
}
