namespace OOP.Encapsulation;

// Subclass overrides the protected property — classic encapsulation + inheritance interplay
public class PremiumAccount : BankAccount
{
    public decimal OverdraftLimit { get; }

    public PremiumAccount(string owner, string accountNumber, decimal overdraftLimit = 500)
        : base(owner, accountNumber)
    {
        OverdraftLimit = overdraftLimit;
    }

    protected override string AccountType => "Premium";
}
