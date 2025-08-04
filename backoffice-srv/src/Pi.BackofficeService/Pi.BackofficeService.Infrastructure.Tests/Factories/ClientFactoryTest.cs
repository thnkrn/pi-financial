using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.WalletService.Model;
using ProductAggregate = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Product;

namespace Pi.BackofficeService.Infrastructure.Tests.Factories;

public class ClientFactoryTest
{
    [Theory]
    [InlineData(ProductAggregate.Funds, PiWalletServiceIntegrationEventsAggregatesModelProduct.Funds)]
    [InlineData(ProductAggregate.TFEX, PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives)]
    [InlineData(ProductAggregate.Cash, PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash)]
    [InlineData(ProductAggregate.Crypto, PiWalletServiceIntegrationEventsAggregatesModelProduct.Crypto)]
    [InlineData(ProductAggregate.CashBalance,
        PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance)]
    [InlineData(ProductAggregate.Margin,
        PiWalletServiceIntegrationEventsAggregatesModelProduct.CreditBalanceSbl)]
    [InlineData(ProductAggregate.GlobalEquity,
        PiWalletServiceIntegrationEventsAggregatesModelProduct.GlobalEquities)]
    public void Should_Return_ExpectedClientProduct_When_NewProduct(ProductAggregate product,
        PiWalletServiceIntegrationEventsAggregatesModelProduct? expected)
    {
        // Act
        var actual = ClientFactory.NewProduct(product);

        // Assert
        Assert.Equal(expected, actual);
    }
}
