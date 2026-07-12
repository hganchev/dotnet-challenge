using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Dtos;
using MinimalApis.Filters;
using MinimalApis.Models;
using MinimalApis.Repositories;

namespace MinimalApis.Endpoints;

/// <summary>
/// Full CRUD Minimal API surface for <see cref="Product"/>, grouped under "/products"
/// with shared tags, OpenAPI metadata, and a logging <see cref="IEndpointFilter"/>.
/// </summary>
public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products")
            .AddEndpointFilter<LoggingEndpointFilter>()
            .WithOpenApi();

        group.MapGet("/", (IProductRepository repository) =>
                TypedResults.Ok(repository.GetAll()))
            .WithName("GetProducts");

        group.MapGet("/{id:int}", Results<Ok<Product>, NotFound> (int id, IProductRepository repository) =>
            {
                var product = repository.GetById(id);
                return product is not null
                    ? TypedResults.Ok(product)
                    : TypedResults.NotFound();
            })
            .WithName("GetProductById");

        group.MapPost("/", ([FromBody] CreateProductRequest request, IProductRepository repository) =>
            {
                var created = repository.Add(request.Name, request.Price);
                return TypedResults.Created($"/products/{created.Id}", created);
            })
            .WithName("CreateProduct");

        group.MapPut("/{id:int}", Results<NoContent, NotFound> (int id, [FromBody] UpdateProductRequest request, IProductRepository repository) =>
            {
                var updated = repository.Update(id, request.Name, request.Price);
                return updated ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("UpdateProduct");

        group.MapDelete("/{id:int}", Results<NoContent, NotFound> (int id, IProductRepository repository) =>
            {
                var deleted = repository.Delete(id);
                return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
            })
            .WithName("DeleteProduct");

        return app;
    }
}
