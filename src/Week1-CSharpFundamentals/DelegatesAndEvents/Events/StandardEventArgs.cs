// Standard .NET pattern: use the built-in EventHandler<TEventArgs> instead of a custom delegate.
// This is the idiomatic approach and is consistent with the BCL (Base Class Library).
public class StandardEventPublisher
{
    // EventHandler<TEventArgs> is a built-in delegate: void (object sender, TEventArgs e)
    public event EventHandler<EventArgs>? StandardEvent;

    public void RaiseStandardEvent()
    {
        StandardEvent?.Invoke(this, EventArgs.Empty);
    }
}

public class StandardEventSubscriber
{
    public void Subscribe(StandardEventPublisher publisher)
    {
        publisher.StandardEvent += OnStandardEvent;
    }

    private void OnStandardEvent(object? sender, EventArgs e)
    {
        Console.WriteLine("StandardEvent received.");
    }
}