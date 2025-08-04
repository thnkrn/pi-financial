using Microsoft.Extensions.Options;
using Moq;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Xunit.Sdk;

namespace Pi.WalletService.Infrastructure.Tests.Services;

public class ValidationServiceTests
{
    private readonly IValidationService _validationService;
    private readonly Mock<IOptionsSnapshot<FeaturesOptions>> _mockFeaturesOptions = new();

    public ValidationServiceTests()
    {
        var featuresOption = new FeaturesOptions
        {
            FreewillOpeningTime = "08:30",
            FreewillClosingTime = "16:30",
            KkpOpeningTime = "00:10",
            KkpClosingTime = "23:55",
            ShouldUseNewErrorCodeOnBankMaintenance = true
        };
        _mockFeaturesOptions.Setup(m => m.Value).Returns(featuresOption);
        _validationService = new ValidationService(_mockFeaturesOptions.Object);
    }

    [Theory]
    [InlineData(Product.GlobalEquities, Channel.ATS, "10:30", false, "", "")]
    [InlineData(Product.GlobalEquities, Channel.ATS, "17:30", true, ErrorCodes.OutsideWorkingHourError, ErrorMessages.OutsideWorkingHourError)]
    [InlineData(Product.GlobalEquities, Channel.OnlineViaKKP, "23:30", false, "", "")]
    [InlineData(Product.GlobalEquities, Channel.OnlineViaKKP, "00:00", true, ErrorCodes.BankMaintenance, ErrorMessages.BankMaintenance)]
    [InlineData(Product.Cash, Channel.ODD, "16:29", false, "", "")]
    [InlineData(Product.CashBalance, Channel.QR, "08:00", true, ErrorCodes.OutsideWorkingHourError, ErrorMessages.OutsideWorkingHourError)]
    [InlineData(Product.Cash, Channel.OnlineViaKKP, "12:30", false, "", "")]
    [InlineData(Product.CashBalance, Channel.OnlineViaKKP, "18:00", true, ErrorCodes.OutsideWorkingHourError, ErrorMessages.OutsideWorkingHourError)]
    public void IsOutSideWorkingHours_Should_ValidateCorrectly(Product product, Channel channel, string time, bool expected, string code, string message)
    {
        var date = DateTime.Today.Add(TimeSpan.Parse(time));
        Assert.Equal(expected, _validationService.IsOutsideWorkingHour(product, channel, date, out var result));
        Assert.Equal(code, result.ErrorCode);
        Assert.Equal(message, result.ErrorMessage);
    }
}