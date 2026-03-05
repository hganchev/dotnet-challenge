// Each demo runs in its own file scope so the Classic and Modern
// using directives never collide with each other.

Console.WriteLine("=== Extension Methods: .NET 9 (C# 13) vs .NET 10 (C# 14) ===");
#if NET10_0_OR_GREATER
Console.WriteLine("  Runtime: .NET 10 — both sections shown");
#else
Console.WriteLine("  Runtime: .NET 9  — only classic section applies");
Console.WriteLine("  Tip: run with -f net10.0 to see the .NET 10 section too");
#endif
Console.WriteLine();

ClassicDemo.Run();
#if NET10_0_OR_GREATER
ModernDemo.Run();
#endif
