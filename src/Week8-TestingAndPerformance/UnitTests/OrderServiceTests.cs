using FluentAssertions;
using Moq;
using Shared.Abstractions;
using Shared.Domain;
using Shared.Services;

namespace UnitTests;

/// <summary>
/// Unit tests for <see cref="OrderService"/>. Both collaborators are mocked so these tests
/// exercise OrderService's own logic (validation, pricing, orchestration) in isolation.
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly Mock<IDiscountService> _discountServiceMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(_repositoryMock.Object, _discountServiceMock.Object);
    }

    [Fact]
    public async Task PlaceOrderAsync_ValidOrder_ReturnsResultWithDiscountedTotal()
    {
        // Arrange
        var order = CreateOrder(quantity: 2, unitPrice: 50m); // Subtotal = 100
        _discountServiceMock.Setup(d => d.GetDiscountPercentage(order)).Returns(10m);

        // Act
        var result = await _sut.PlaceOrderAsync(order);

        // Assert
        result.Subtotal.Should().Be(100m);
        result.DiscountPercentage.Should().Be(10m);
        result.Total.Should().Be(90m);
    }

    [Fact]
    public async Task PlaceOrderAsync_ValidOrder_SavesOrderExactlyOnce()
    {
        // Arrange
        var order = CreateOrder(quantity: 1, unitPrice: 20m);
        _discountServiceMock.Setup(d => d.GetDiscountPercentage(order)).Returns(0m);

        // Act
        await _sut.PlaceOrderAsync(order);

        // Assert
        _repositoryMock.Verify(r => r.SaveAsync(order), Times.Once);
    }

    [Fact]
    public async Task PlaceOrderAsync_NoDiscountFromDiscountService_ReturnsFullSubtotalAsTotal()
    {
        // Arrange
        var order = CreateOrder(quantity: 3, unitPrice: 10m); // Subtotal = 30
        _discountServiceMock.Setup(d => d.GetDiscountPercentage(order)).Returns(0m);

        // Act
        var result = await _sut.PlaceOrderAsync(order);

        // Assert
        result.Total.Should().Be(30m);
    }

    [Fact]
    public async Task PlaceOrderAsync_OrderWithNoItems_ThrowsArgumentException()
    {
        // Arrange
        var order = new Order { Customer = "Ada Lovelace" };

        // Act
        var act = () => _sut.PlaceOrderAsync(order);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task PlaceOrderAsync_ItemWithZeroQuantity_ThrowsArgumentException()
    {
        // Arrange
        var order = CreateOrder(quantity: 0, unitPrice: 10m);

        // Act
        var act = () => _sut.PlaceOrderAsync(order);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }

    private static Order CreateOrder(int quantity, decimal unitPrice) => new()
    {
        Customer = "Ada Lovelace",
        Items =
        [
            new OrderItem { ProductName = "Widget", Quantity = quantity, UnitPrice = unitPrice }
        ]
    };
}
