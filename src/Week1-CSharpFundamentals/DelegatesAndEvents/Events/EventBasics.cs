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
