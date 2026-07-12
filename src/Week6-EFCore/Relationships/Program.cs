using Microsoft.EntityFrameworkCore;
using Relationships.Concepts;
using Relationships.Data;
using Relationships.Entities;

Console.WriteLine("Week 6 - EF Core Relationships");
Console.WriteLine("===============================\n");

var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>()
    .UseSqlite("Data Source=library.db");

await using var context = new LibraryDbContext(optionsBuilder.Options);
await context.Database.MigrateAsync();
await SeedAsync(context);

await OneToOneDemo.RunAsync(context);
await OneToManyDemo.RunAsync(context);
await ManyToManyDemo.RunAsync(context);
await OwnedTypeDemo.RunAsync(context);
await CascadeDeleteDemo.RunAsync(context);

static async Task SeedAsync(LibraryDbContext context)
{
    if (await context.Authors.AnyAsync())
    {
        return;
    }

    var scienceTag = new Tag { Name = "Science" };
    var historyTag = new Tag { Name = "History" };

    var author = new Author
    {
        Name = "Ada Lovelace",
        Profile = new AuthorProfile
        {
            Bio = "Mathematician and writer, first computer programmer.",
            Address = new Address { Street = "12 Mayfair", City = "London", PostalCode = "W1J" }
        },
        Books =
        {
            new Book
            {
                Title = "Notes on the Analytical Engine",
                BookTags =
                {
                    new BookTag { Tag = scienceTag, AddedAt = DateTime.UtcNow },
                    new BookTag { Tag = historyTag, AddedAt = DateTime.UtcNow }
                }
            }
        }
    };

    context.Authors.Add(author);
    await context.SaveChangesAsync();
}

