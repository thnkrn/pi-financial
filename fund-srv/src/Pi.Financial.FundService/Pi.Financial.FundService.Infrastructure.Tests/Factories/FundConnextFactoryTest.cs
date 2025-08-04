using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.Factories;
using static Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Infrastructure.Tests.Factories;

public class FundConnextFactoryTest
{
    [Fact]
    public void Should_Return_ApplicationFundConnext_With_Expected()
    {
        // Arrange
        var input = FakeFundOrder();

        // Act
        var actual = ApplicationFactory.NewFundOrder(input);

        // Assert
        Assert.IsType<Application.Models.FundOrder>(actual);
        Assert.Equal(input.Unit, actual.Unit);
        Assert.Equal(input.Amount, actual.Amount);
        Assert.Equal(input.BankCode, actual.BankCode);
        Assert.Equal(input.AccountId, actual.AccountId);
        Assert.Equal(input.FundCode, actual.FundCode);
        Assert.Equal(input.TransactionId, actual.BrokerOrderId);
        Assert.Null(actual.Edd);
        Assert.Null(actual.SwitchIn);
        Assert.Null(actual.SwitchTo);
    }

    [Theory]
    [InlineData("TRN_SA", "TRN")]
    [InlineData("ATS_SA", "ATS")]
    public void Should_Return_ApplicationFundConnext_With_ExpectedPaymentMethod(string paymentType, string expected)
    {
        // Arrange
        var input = FakeFundOrder();
        input.PaymentType = paymentType;

        // Act
        var actual = ApplicationFactory.NewFundOrder(input);

        // Assert
        Assert.IsType<Application.Models.FundOrder>(actual);
        Assert.Equal(expected, actual.PaymentMethod);
    }

    [Theory]
    [InlineData(StatusEnum.APPROVED, FundOrderStatus.Approved)]
    [InlineData(StatusEnum.WAITING, FundOrderStatus.Waiting)]
    [InlineData(StatusEnum.REJECTED, FundOrderStatus.Rejected)]
    [InlineData(StatusEnum.CANCELLED, FundOrderStatus.Cancelled)]
    [InlineData(StatusEnum.EXPIRED, FundOrderStatus.Expired)]
    [InlineData(StatusEnum.ALLOTTED, FundOrderStatus.Allotted)]
    [InlineData(StatusEnum.SUBMITTED, FundOrderStatus.Submitted)]
    public void Should_Return_ApplicationFundConnext_With_ExpectedStatus(StatusEnum status, FundOrderStatus expected)
    {
        // Arrange
        var input = FakeFundOrder();
        input.Status = status;

        // Act
        var actual = ApplicationFactory.NewFundOrder(input);

        // Assert
        Assert.IsType<Application.Models.FundOrder>(actual);
        Assert.Equal(expected, actual.Status);
    }

    [Theory]
    [InlineData("SUB", FundOrderType.Subscription)]
    [InlineData("RED", FundOrderType.Redemption)]
    [InlineData("SWO", FundOrderType.SwitchOut)]
    [InlineData("SWI", FundOrderType.SwitchIn)]
    [InlineData("TRO", FundOrderType.TransferOut)]
    [InlineData("TRI", FundOrderType.TransferIn)]
    public void Should_Return_ApplicationFundConnext_With_ExpectedOrderType(string orderType, FundOrderType expected)
    {
        // Arrange
        var input = FakeFundOrder();
        input.OrderType = orderType;

        // Act
        var actual = ApplicationFactory.NewFundOrder(input);

        // Assert
        Assert.IsType<Application.Models.FundOrder>(actual);
        Assert.Equal(expected, actual.OrderType);
    }

    [Fact]
    public void Should_Return_ApplicationFundConnext_With_ExpectedDateOrDateTimeFields()
    {
        // Arrange
        var input = FakeFundOrder();
        input.TransactionLastUpdated = "20231218141257";
        input.EffectiveDate = "20231218";
        input.TransactionDateTime = "20231218151257";


        // Act
        var actual = ApplicationFactory.NewFundOrder(input);

        // Assert
        Assert.IsType<Application.Models.FundOrder>(actual);
        Assert.Equal(new DateTime(2023, 12, 18, 14, 12, 57, DateTimeKind.Unspecified), actual.TransactionLastUpdated);
        Assert.Equal(new DateTime(2023, 12, 18, 15, 12, 57, DateTimeKind.Unspecified), actual.TransactionDateTime);
        Assert.Equal(new DateOnly(2023, 12, 18), actual.EffectiveDate);
    }

    private static FundOrder FakeFundOrder()
    {
        return new FundOrder("1672312180000831",
        "374113",
        "SUB",
        "77112531",
        "9390103768",
        "KFGTECH-A",
        "UNIT",
        100,
        500,
        null,
        StatusEnum.EXPIRED,
        "20231218151257",
        "20231218",
        "20231218",
        null,
        100,
        100,
        100,
        "20231218",
        100,
        "20231218141257",
        "ATS_SA",
        "025",
        "0019148331",
        "MOB",
        "028773",
        "00",
        "N",
        "",
        "",
        null,
        null,
        "20231219",
        "",
        "OMN");
    }
}
