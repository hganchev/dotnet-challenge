using MessagingAndEventDrivenArchitecture.Examples;

await PubSubDemo.RunAsync();
Console.WriteLine();
await OutboxDemo.RunAsync();
Console.WriteLine();
EventSourcingDemo.Run();
Console.WriteLine();
DeliveryGuaranteesDemo.Run();
