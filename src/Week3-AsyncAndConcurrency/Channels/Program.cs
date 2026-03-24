using Channels.Examples;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   Week 3 — Channels                     ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine();

await BasicChannelDemo.RunAsync();
await ProducerConsumerDemo.RunAsync();
await MultiProducerConsumerDemo.RunAsync();
await PipelineDemo.RunAsync();
await BackPressureDemo.RunAsync();

Console.WriteLine("═══════ All demos complete ═══════");

