namespace CommonPatterns.FactoryMethod;

/// <summary>Product abstraction — callers depend on this, never on concrete notification types.</summary>
public interface INotification
{
    string Send(string recipient, string message);
}

public sealed class EmailNotification : INotification
{
    public string Send(string recipient, string message) => $"  [Email] To: {recipient} | {message}";
}

public sealed class SmsNotification : INotification
{
    public string Send(string recipient, string message) => $"  [SMS] To: {recipient} | {message}";
}

public sealed class PushNotification : INotification
{
    public string Send(string recipient, string message) => $"  [Push] To: {recipient} | {message}";
}

public enum NotificationChannel
{
    Email,
    Sms,
    Push
}

/// <summary>Factory Method — decouples the caller from the concrete <see cref="INotification"/>
/// type; adding a new channel only requires extending this factory, not the call sites.</summary>
public static class NotificationFactory
{
    public static INotification Create(NotificationChannel channel) => channel switch
    {
        NotificationChannel.Email => new EmailNotification(),
        NotificationChannel.Sms => new SmsNotification(),
        NotificationChannel.Push => new PushNotification(),
        _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, "Unknown notification channel.")
    };
}
