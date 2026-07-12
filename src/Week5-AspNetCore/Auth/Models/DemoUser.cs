namespace Auth.Models;

/// <summary>Hardcoded demo user store — replace with a real identity provider.</summary>
public sealed record DemoUser(string Username, string Password, string Role, int Age);

public static class DemoUserStore
{
    public static readonly IReadOnlyList<DemoUser> Users =
    [
        new DemoUser("admin", "admin123", Role: "Admin", Age: 35),
        new DemoUser("alice", "alice123", Role: "User", Age: 28),
        new DemoUser("teen", "teen123", Role: "User", Age: 15)
    ];

    public static DemoUser? Validate(string username, string password) =>
        Users.SingleOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
}
