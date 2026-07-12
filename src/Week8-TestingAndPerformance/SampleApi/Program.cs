using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SampleApi.Data;
using SampleApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Registered here (rather than only in tests) so `dotnet run` also works standalone.
// Integration tests override this registration with their own isolated in-memory database name.
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("SampleApiDb"));

var app = builder.Build();

var products = app.MapGroup("/products");

products.MapGet("/", async (ProductDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync());

products.MapGet("/{id:int}", async Task<Results<Ok<Product>, NotFound>> (int id, ProductDbContext db) =>
    await db.Products.FindAsync(id) is { } product
        ? TypedResults.Ok(product)
        : TypedResults.NotFound());

products.MapPost("/", async (Product product, ProductDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/products/{product.Id}", product);
});

products.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, Product input, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
    {
        return TypedResults.NotFound();
    }

    product.Name = input.Name;
    product.Price = input.Price;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
});

products.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
    {
        return TypedResults.NotFound();
    }

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
});

app.Run();

/// <summary>Exposes the implicit entry-point Program type so
/// <c>WebApplicationFactory&lt;Program&gt;</c> can host this app in-process for integration tests.</summary>
public partial class Program { }
