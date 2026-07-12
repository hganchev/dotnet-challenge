using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SampleApi.Models;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the <c>/products</c> endpoints. These exercise the full request
/// pipeline (routing, model binding, DI, EF Core) over a real in-process HTTP client rather
/// than calling handler methods directly.
/// </summary>
public class ProductsApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostProduct_ValidProduct_ReturnsCreatedWithLocationHeader()
    {
        // Arrange
        var product = new Product { Name = "Keyboard", Price = 49.99m };

        // Act
        var response = await _client.PostAsJsonAsync("/products", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsOkWithProduct()
    {
        // Arrange
        var created = await CreateProductAsync("Mouse", 19.99m);

        // Act
        var response = await _client.GetAsync($"/products/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await response.Content.ReadFromJsonAsync<Product>();
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Mouse");
    }

    [Fact]
    public async Task GetProductById_MissingProduct_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/products/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllProducts_AfterCreatingProduct_ContainsThatProduct()
    {
        // Arrange
        var created = await CreateProductAsync("Monitor", 249.00m);

        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var allProducts = await response.Content.ReadFromJsonAsync<List<Product>>();
        allProducts.Should().NotBeNull();
        allProducts!.Should().Contain(p => p.Id == created.Id && p.Name == "Monitor");
    }

    [Fact]
    public async Task PutProduct_ExistingProduct_UpdatesFieldsAndReturnsNoContent()
    {
        // Arrange
        var created = await CreateProductAsync("Webcam", 59.99m);
        var update = new Product { Id = created.Id, Name = "Webcam Pro", Price = 79.99m };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"/products/{created.Id}", update);
        var getResponse = await _client.GetAsync($"/products/{created.Id}");

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var updated = await getResponse.Content.ReadFromJsonAsync<Product>();
        updated!.Name.Should().Be("Webcam Pro");
        updated.Price.Should().Be(79.99m);
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_RemovesItSoSubsequentGetReturnsNotFound()
    {
        // Arrange
        var created = await CreateProductAsync("Headset", 89.00m);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/products/{created.Id}");
        var getResponse = await _client.GetAsync($"/products/{created.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Product> CreateProductAsync(string name, decimal price)
    {
        var response = await _client.PostAsJsonAsync("/products", new Product { Name = name, Price = price });
        var created = await response.Content.ReadFromJsonAsync<Product>();
        return created!;
    }
}
