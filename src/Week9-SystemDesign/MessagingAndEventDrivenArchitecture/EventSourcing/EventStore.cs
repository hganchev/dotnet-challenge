namespace MessagingAndEventDrivenArchitecture.EventSourcing;

/// <summary>An append-only store of every event that has ever happened, per account.</summary>
public class EventStore
{
    private readonly List<AccountEvent> _events = [];

    public void Append(AccountEvent evt) => _events.Add(evt);

    public IReadOnlyList<AccountEvent> GetEventsFor(Guid accountId) =>
        _events.Where(e => e.AccountId == accountId).ToList();
}
