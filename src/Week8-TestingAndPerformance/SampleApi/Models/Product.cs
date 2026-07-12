namespace SampleApi.Models;

/// <summary>A catalog product exposed by the sample API used for integration testing.</summary>
public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public decimal Price { get; set; }
}
