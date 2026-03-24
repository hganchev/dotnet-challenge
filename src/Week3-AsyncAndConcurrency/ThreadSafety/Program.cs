using ThreadSafety.Examples;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Week 3 — Thread Safety                ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine();

LockDemo.Run();
InterlockedDemo.Run();
ConcurrentCollectionsDemo.Run();
await SemaphoreSlimDemo.RunAsync();
ReaderWriterDemo.Run();
ImmutabilityDemo.Run();

Console.WriteLine("═══════ All demos complete ═══════");
