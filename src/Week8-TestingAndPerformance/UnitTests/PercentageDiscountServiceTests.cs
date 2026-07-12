using FluentAssertions;
using Shared.Domain;
using Shared.Services;

namespace UnitTests;

/// <summary>Unit tests for the concrete <see cref="PercentageDiscountService"/> — no mocking
/// needed here since it has no dependencies, just pure input/output behaviour to verify.</summary>
public class PercentageDiscountServiceTests
{
    private readonly PercentageDiscountService _sut = new();

    [Fact]
    public void GetDiscountPercentage_SubtotalBelowThreshold_ReturnsZero()
    {
        // Arrange
        var order = CreateOrderWithSubtotal(99m);

        // Act
        var discount = _sut.GetDiscountPercentage(order);

        // Assert
        discount.Should().Be(0m);
    }

    [Fact]
    public void GetDiscountPercentage_SubtotalAtThreshold_ReturnsTenPercent()
    {
        // Arrange
        var order = CreateOrderWithSubtotal(100m);

        // Act
        var discount = _sut.GetDiscountPercentage(order);

        // Assert
        discount.Should().Be(10m);
    }

    [Fact]
    public void GetDiscountPercentage_SubtotalAboveThreshold_ReturnsTenPercent()
    {
        // Arrange
        var order = CreateOrderWithSubtotal(250m);

        // Act
        var discount = _sut.GetDiscountPercentage(order);

        // Assert
        discount.Should().Be(10m);
    }

    private static Order CreateOrderWithSubtotal(decimal subtotal) => new()
    {
        Customer = "Grace Hopper",
        Items = [new OrderItem { ProductName = "Widget", Quantity = 1, UnitPrice = subtotal }]
    };
}
