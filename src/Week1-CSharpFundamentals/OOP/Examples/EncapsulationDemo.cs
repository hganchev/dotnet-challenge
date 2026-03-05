using OOP.Encapsulation;

namespace OOP.Examples;

public static class EncapsulationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Encapsulation & Access Modifiers ===");

        // Constructor sets Owner (init-only); cannot reassign after this point
        var account = new BankAccount("Alice", "ACC-001", 200m);
        // account.Owner = "Eve";  // compile error: init-only property

        account.Deposit(150m);
        account.Withdraw(50m);
        Console.WriteLine(account);

        bool denied = account.Withdraw(500m);  // exceeds balance
        Console.WriteLine($"Withdraw £500: {(denied ? "ok" : "denied")}");

        Console.WriteLine("Transaction log:");
        foreach (var t in account.Transactions)
            Console.WriteLine($"  {t}");

        // PremiumAccount: overrides protected AccountType property
        var premium = new PremiumAccount("Bob", "ACC-002");
        premium.Deposit(1_000m);
        Console.WriteLine($"\n{premium}  (overdraft limit: {premium.OverdraftLimit:C})");

        // PersonInfo: demonstrates all-public init-only setters via object initializer
        Console.WriteLine("\n--- init-only properties ---");
        var person = new PersonInfo
        {
            FirstName = "Carol",
            LastName  = "White",
            Age       = 32,
            Email     = "carol@example.com"
        };
        Console.WriteLine(person);
        person.Age = 33;  // regular set is fine after construction
        Console.WriteLine($"Birthday: now age {person.Age}");
    }
}
