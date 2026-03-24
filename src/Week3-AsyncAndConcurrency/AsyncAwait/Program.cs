using AsyncAwait.Concepts;
using AsyncAwait.Examples;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Week 3 — Async / Await                ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine();

await BasicAsyncDemo.RunAsync();
await ConfigureAwaitDemo.RunAsync();
await CancellationDemo.RunAsync();
await WhenAllWhenAnyDemo.RunAsync();
await AsyncStreamDemo.RunAsync();
await ValueTaskDemo.RunAsync();
await AsyncStateMachineExplained.RunAsync();

Console.WriteLine("═══════ All demos complete ═══════");
