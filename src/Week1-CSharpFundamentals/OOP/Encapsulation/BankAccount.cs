namespace OOP.Encapsulation;

// Demonstrates encapsulation: hide internals, expose a minimal public API.
// Also showcases all five access modifiers and property variants.
public class BankAccount
{
    // private: only this class can read/write
    private decimal _balance;
    private readonly string _accountNumber;
    private readonly List<string> _transactions = [];

    // public + init-only setter: settable only during construction / object-initializer
    public string Owner { get; init; }

    // Expression-bodied read-only properties (hide backing fields)
    public string AccountNumber => _accountNumber;
    public decimal Balance => _balance;
    public IReadOnlyList<string> Transactions => _transactions.AsReadOnly();

    public BankAccount(string owner, string accountNumber, decimal initialBalance = 0)
    {
        Owner = owner;
        _accountNumber = accountNumber;
        _balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit amount must be positive.");
        _balance += amount;
        _transactions.Add($"Deposit: +{amount:C}");
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0 || amount > _balance) return false;
        _balance -= amount;
        _transactions.Add($"Withdrawal: -{amount:C}");
        return true;
    }

    // protected: accessible to this class and subclasses only
    protected virtual string AccountType => "Standard";

    // internal: accessible within the same assembly
    internal string InternalRef => $"{_accountNumber}:{_balance:F2}";

    public override string ToString() => $"[{AccountType}] {Owner} ({_accountNumber}): {_balance:C}";
}

