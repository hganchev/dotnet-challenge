namespace OOP.StaticMembers;

// Thread-safe lazy singleton: one instance for the whole application lifetime.
// sealed prevents subclassing from accidentally creating a second instance.
public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _instance =
        new(() => new AppSettings(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static AppSettings Instance => _instance.Value;

    public string AppName       { get; private set; } = "DotNetChallenge";
    public string Version       { get; private set; } = "1.0.0";
    public bool   IsDevelopment { get; private set; } = true;

    // private constructor: prevents external instantiation
    private AppSettings() { }

    public void Configure(string appName, string version)
    {
        AppName  = appName;
        Version  = version;
    }

    public override string ToString() => $"{AppName} v{Version} (dev={IsDevelopment})";
}

// Static class: cannot be instantiated or subclassed; all members must be static.
public static class MathUtils
{
    public static double DegreesToRadians(double deg) => deg * Math.PI / 180;
    public static double RadiansToDegrees(double rad) => rad * 180 / Math.PI;

    public static bool IsPrime(int n) =>
        n > 1 && Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i != 0);

    public static int Fibonacci(int n) => n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
}
