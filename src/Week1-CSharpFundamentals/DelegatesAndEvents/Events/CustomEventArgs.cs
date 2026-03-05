namespace DelegatesAndEvents.Events;

public class CustomEventArgs : EventArgs
{
    public string Message { get; }

    public CustomEventArgs(string message)
    {
        Message = message;
    }
}

public class CustomEventPublisher
{
    // Define a delegate type for the event
    public delegate void CustomEventHandler(object sender, CustomEventArgs e);

    // Define an event based on the delegate
    public event CustomEventHandler? CustomEvent;

    // Method to raise the event
    public void RaiseCustomEvent(string message)
    {
        CustomEvent?.Invoke(this, new CustomEventArgs(message)); // Invoke the event with custom event args
    }
}

public class CustomEventSubscriber
{
    public void Subscribe(CustomEventPublisher publisher)
    {
        publisher.CustomEvent += OnCustomEvent;
    }

    private void OnCustomEvent(object? sender, CustomEventArgs e)
    {
        Console.WriteLine($"CustomEvent received: {e.Message}");
    }
}

// Standard .NET pattern: use the built-in EventHandler<TEventArgs> instead of a custom delegate.
// This is the idiomatic approach and is consistent with the BCL (Base Class Library).
public class StandardEventPublisher
{
    // EventHandler<TEventArgs> is a built-in delegate: void (object sender, TEventArgs e)
    public event EventHandler<CustomEventArgs>? StandardEvent;

    public void RaiseStandardEvent(string message)
    {
        StandardEvent?.Invoke(this, new CustomEventArgs(message));
    }
}

public class StandardEventSubscriber
{
    public void Subscribe(StandardEventPublisher publisher)
    {
        publisher.StandardEvent += OnStandardEvent;
    }

    private void OnStandardEvent(object? sender, CustomEventArgs e)
    {
        Console.WriteLine($"StandardEvent received: {e.Message}");
    }
}