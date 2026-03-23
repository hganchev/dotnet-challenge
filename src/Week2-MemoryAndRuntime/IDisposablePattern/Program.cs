using IDisposablePattern.Examples;

Console.WriteLine("=== Week 2 — IDisposable Pattern ===");

// --- Simple IDisposable: sealed class owning managed resources ---
SimpleDisposeDemo.Run();

// --- Full dispose pattern: unsealed class + derived class ---
FullDisposePatternDemo.Run();

// --- using statement vs using declaration, Dispose on exception ---
UsingStatementDemo.Run();

// --- SafeHandle: preferred wrapper for native handles ---
SafeHandleDemo.Run();

// --- ObjectDisposedException: guarding public API after disposal ---
ObjectDisposedDemo.Run();
