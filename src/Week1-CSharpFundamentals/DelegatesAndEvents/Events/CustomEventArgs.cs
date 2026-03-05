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