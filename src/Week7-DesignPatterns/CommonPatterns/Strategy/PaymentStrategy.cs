namespace CommonPatterns.Strategy;

/// <summary>Strategy abstraction — a family of interchangeable algorithms behind one interface.</summary>
public interface IPaymentStrategy
{
    string Pay(decimal amount);
}

public sealed class CreditCardPayment(string cardNumber) : IPaymentStrategy
{
    public string Pay(decimal amount) => $"  Paid {amount:C} with credit card ending in {cardNumber[^4..]}.";
}

public sealed class PayPalPayment(string email) : IPaymentStrategy
{
    public string Pay(decimal amount) => $"  Paid {amount:C} via PayPal account {email}.";
}

public sealed class CryptoPayment(string walletAddress) : IPaymentStrategy
{
    public string Pay(decimal amount) => $"  Paid {amount:C} via crypto wallet {walletAddress[..8]}…";
}

/// <summary>Context — holds a reference to the current strategy, swappable at runtime without
/// any conditional logic here.</summary>
public sealed class PaymentProcessor(IPaymentStrategy strategy)
{
    public IPaymentStrategy Strategy { get; set; } = strategy;

    public string ProcessPayment(decimal amount) => Strategy.Pay(amount);
}
