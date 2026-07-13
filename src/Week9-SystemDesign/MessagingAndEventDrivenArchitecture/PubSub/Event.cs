namespace MessagingAndEventDrivenArchitecture.PubSub;

/// <summary>A domain event flowing through the pub/sub bus.</summary>
public record Event(Guid Id, string Type, string Payload, DateTimeOffset OccurredAt)
{
    public static Event Create(string type, string payload) =>
        new(Guid.NewGuid(), type, payload, DateTimeOffset.UtcNow);
}
