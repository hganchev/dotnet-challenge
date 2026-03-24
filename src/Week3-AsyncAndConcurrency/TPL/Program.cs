using TPL.Examples;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Week 3 — Task Parallel Library (TPL)  ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine();

await TaskRunDemo.RunAsync();
await ParallelForDemo.RunAsync();
PlinqDemo.Run();
ParallelInvokeDemo.Run();
await TaskCompletionSourceDemo.RunAsync();
await AggregateExceptionDemo.RunAsync();

Console.WriteLine("═══════ All demos complete ═══════");
