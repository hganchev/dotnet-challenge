namespace MessagingAndEventDrivenArchitecture.EventSourcing;

/// <summary>An account whose current state is never stored directly — it's always derived by
/// replaying its full event history. The events are the source of truth, not the balance field.</summary>
public class Account
{
    public Guid Id { get; }

    public decimal Balance { get; private set; }

    private Account(Guid id) => Id = id;

    public static Account Replay(Guid id, IEnumerable<AccountEvent> events)
    {
        var account = new Account(id);
        foreach (var evt in events)
        {
            account.Apply(evt);
        }

        return account;
    }

    private void Apply(AccountEvent evt)
    {
        switch (evt)
        {
            case FundsDeposited deposited:
                Balance += deposited.Amount;
                break;
            case FundsWithdrawn withdrawn:
                Balance -= withdrawn.Amount;
                break;
        }
    }
}
