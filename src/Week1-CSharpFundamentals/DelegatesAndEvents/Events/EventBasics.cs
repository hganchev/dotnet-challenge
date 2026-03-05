namespace DelegatesAndEvents.Events;

public class EventBasics
{
    // Define a delegate type for the event
    public delegate void MyEventHandler(string message);

    // Define an event based on the delegate
    public event MyEventHandler? MyEvent;

    // Method to raise the event
    public void RaiseEvent(string message)
    {
        MyEvent?.Invoke(message); // Invoke the event if there are subscribers
    }

    public void Run()
    {
        // Subscribe to the event with a handler method
        MyEvent += HandleMyEvent;

        // Raise the event with a message
        RaiseEvent("Hello, this is an event!"); // Output: Event received: Hello, this is an event!

        // Unsubscribe from the event
        MyEvent -= HandleMyEvent;

        // Raise the event again (no output since there are no subscribers)
        RaiseEvent("This will not be handled.");
    }

    // Event handler method that matches the delegate signature
    private void HandleMyEvent(string message)
    {
        Console.WriteLine($"Event received: {message}");
    }
}

// Demonstrates event memory leaks.
// A publisher holds a strong reference to every subscriber via its event backing field.
// If a subscriber forgets to unsubscribe, the GC cannot collect it even when all other
// references to it are gone — because the publisher still holds one through the event.
public class EventMemoryLeakDemo
{
    public event EventHandler? LongLivedEvent;

    public void Run()
    {
        var subscriber = new Subscriber();

        // Subscribe — publisher now holds a strong reference to 'subscriber'
        subscriber.Subscribe(this);

        // Fire the event
        LongLivedEvent?.Invoke(this, EventArgs.Empty); // Output: Subscriber handled the event

        // Fix: always unsubscribe when the subscriber is done
        subscriber.Unsubscribe(this);

        // After unsubscribing, 'subscriber' can be garbage collected normally
        LongLivedEvent?.Invoke(this, EventArgs.Empty); // No output — no subscribers
    }

    private class Subscriber
    {
        public void Subscribe(EventMemoryLeakDemo publisher)
        {
            publisher.LongLivedEvent += OnEvent;
        }

        public void Unsubscribe(EventMemoryLeakDemo publisher)
        {
            publisher.LongLivedEvent -= OnEvent; // Release the strong reference
        }

        private void OnEvent(object? sender, EventArgs e)
        {
            Console.WriteLine("Subscriber handled the event");
        }
    }
}