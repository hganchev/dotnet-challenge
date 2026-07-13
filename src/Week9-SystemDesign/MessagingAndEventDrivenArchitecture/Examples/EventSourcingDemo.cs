using MessagingAndEventDrivenArchitecture.EventSourcing;

namespace MessagingAndEventDrivenArchitecture.Examples;

/// <summary>Demonstrates event sourcing basics: state is stored as a sequence of events, and
/// current state is rebuilt on demand by replaying them.</summary>
public static class EventSourcingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Event Sourcing Basics ===");
        var accountId = Guid.NewGuid();
        var store = new EventStore();

        store.Append(new FundsDeposited(accountId, 100m, DateTimeOffset.UtcNow));
        store.Append(new FundsDeposited(accountId, 50m, DateTimeOffset.UtcNow));
        store.Append(new FundsWithdrawn(accountId, 30m, DateTimeOffset.UtcNow));

        var events = store.GetEventsFor(accountId);
        var account = Account.Replay(accountId, events);

        Console.WriteLine($"Rebuilt account balance after replaying {events.Count} events: {account.Balance:C}");
    }
}
