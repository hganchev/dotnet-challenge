using Fundamentals.Concepts;
using Fundamentals.Data;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Week 6 - EF Core Fundamentals");
Console.WriteLine("==============================\n");

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("Data Source=fundamentals.db");

await using var context = new AppDbContext(optionsBuilder.Options);

await MigrationsDemo.RunAsync(context);
ConventionsDemo.Run(context);
DataAnnotationsDemo.Run(context);
FluentApiDemo.Run(context);
await ConcurrencyAndTransactionsDemo.RunAsync(context);

