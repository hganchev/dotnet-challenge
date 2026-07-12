namespace MinimalApis.Dtos;

/// <summary>Request body for creating a product. Bound via <c>[FromBody]</c>.</summary>
public sealed record CreateProductRequest(string Name, decimal Price);

/// <summary>Request body for updating a product. Bound via <c>[FromBody]</c>.</summary>
public sealed record UpdateProductRequest(string Name, decimal Price);
