using MessagingAndEventDrivenArchitecture.PubSub;

namespace MessagingAndEventDrivenArchitecture.DeliveryGuarantees;

/// <summary>Consumes events idempotently by deduplicating on event id. Simulates a consumer sitting
/// behind an at-least-once transport that may redeliver the same message.</summary>
public class IdempotentConsumer
{
    private readonly HashSet<Guid> _processedEventIds = [];

    public int ProcessedCount { get; private set; }

    public void Consume(Event evt)
    {
        if (!_processedEventIds.Add(evt.Id))
        {
            Console.WriteLine($"  [IdempotentConsumer] duplicate delivery of {evt.Id} ignored");
            return;
        }

        ProcessedCount++;
        Console.WriteLine($"  [IdempotentConsumer] processed {evt.Type} ({evt.Id})");
    }
}
