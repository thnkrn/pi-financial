using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Tests.Factories;

public class EntityFactoryTest
{
    [Fact]
    public void Should_Return_User_When_NewUser_With_UserUpdateOrCreateRequest()
    {
        // Arrange
        var payload = new UserUpdateOrCreateRequest(Guid.NewGuid(), "FirstName", "LastName", "email");

        // Act
        var actual = EntityFactory.NewUser(payload);

        // Assert
        Assert.IsType<User>(actual);
    }

    [Theory]
    [InlineData(TransactionType.Deposit, Machine.Deposit)]
    [InlineData(TransactionType.Withdraw, Machine.Withdraw)]
    public void Should_Return_ExpectedMachine_When_NewMachine_With_TransactionType(TransactionType transactionType, Machine machine)
    {
        // Act
        var actual = EntityFactory.NewMachine(transactionType);

        // Assert
        Assert.Equal(machine, actual);
    }

    [Theory]
    [InlineData(Product.GlobalEquity, ProductType.GlobalEquity)]
    [InlineData(Product.Cash, ProductType.ThaiEquity)]
    [InlineData(Product.Funds, ProductType.ThaiEquity)]
    [InlineData(Product.Margin, ProductType.ThaiEquity)]
    [InlineData(Product.CashBalance, ProductType.ThaiEquity)]
    [InlineData(Product.TFEX, ProductType.ThaiEquity)]
    public void Should_Return_ExpectedProductType_When_NewProductType_With_Product(Product product, ProductType? expected)
    {
        // Act
        var actual = EntityFactory.NewProductType(product);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Product.Crypto)]
    public void Should_Throw_ArgumentOutOfRangeException_When_NewProductType_With_UnmatchedProduct(Product product)
    {
        // Act
        var action = () => EntityFactory.NewProductType(product);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => action());
    }
}
