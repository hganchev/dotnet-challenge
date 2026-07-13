namespace MessagingAndEventDrivenArchitecture.EventSourcing;

/// <summary>Base type for all events that can happen to an <see cref="Account"/>.</summary>
public abstract record AccountEvent(Guid AccountId, DateTimeOffset OccurredAt);

public record FundsDeposited(Guid AccountId, decimal Amount, DateTimeOffset OccurredAt)
    : AccountEvent(AccountId, OccurredAt);

public record FundsWithdrawn(Guid AccountId, decimal Amount, DateTimeOffset OccurredAt)
    : AccountEvent(AccountId, OccurredAt);
