using ResilienceAndRateLimiting.Examples;

await PollyResilienceDemo.RunAsync();
Console.WriteLine();
RateLimitingDemo.Run();
Console.WriteLine();
IdempotencyDemo.Run();
