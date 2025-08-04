using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Client.ItBackOffice.Api;
using Pi.Client.ItBackOffice.Client;
using Pi.Client.ItBackOffice.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Services;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class ItServiceTests
{
    private readonly Mock<IBackOfficeApi> _backOfficeApi;
    private readonly ItService _itService;

    public ItServiceTests()
    {
        Mock<ILogger<ItService>> logger = new();
        Mock<IOptionsSnapshot<ItOptions>> itOptions = new();

        _backOfficeApi = new Mock<IBackOfficeApi>();

        var options = new ItOptions()
        {
            ApiKey = Guid.NewGuid(),
            Host = "MOCK_HOST",
        };

        itOptions.Setup(o => o.Value).Returns(options);

        _itService = new ItService(
            _backOfficeApi.Object,
            itOptions.Object,
            logger.Object);
    }

    [Fact]
    public async void GetTradeDetail_ShouldThrowExceptionWhenCallApiFailed()
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            1,
            2,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"));

        const int errorCode = 500;
        const string errorMessage = "Internal Server Error";

        _backOfficeApi.Setup(s => s.ApiBackOfficeOrderTradesGetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException(errorCode, errorMessage, $@"{{ 'message': '{errorMessage}', 'code': '{errorCode}' }}"));

        // Act
        var exception = await Assert.ThrowsAsync<ItApiException>(async () =>
            await _itService.GetTradeDetail(requestModel, CancellationToken.None));

        // Assert
        Assert.Equal($"ItService GetTradeDetail: {errorMessage}", exception.Message);
    }

    [Fact]
    public async void GetTradeDetail_ShouldThrowExceptionWhenCallApiNotFound()
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            1,
            2,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"));

        const int errorCode = 404;
        const string errorMessage = "Not Found";

        _backOfficeApi.Setup(s => s.ApiBackOfficeOrderTradesGetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException(errorCode, errorMessage, $@"{{ 'message': '{errorMessage}', 'code': '{errorCode}' }}"));

        // Act
        var exception = await Assert.ThrowsAsync<ItNotFoundException>(async () =>
            await _itService.GetTradeDetail(requestModel, CancellationToken.None));

        // Assert
        Assert.Equal($"ItService GetTradeDetail: Trade not found", exception.Message);
    }

    [Fact]
    public async void GetTradeDetail_ShouldThrowExceptionWhenResponseIsNotSuccess()
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            1,
            2,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"),
            Side.Long,
            Position.Auto);

        _backOfficeApi.Setup(s => s.ApiBackOfficeOrderTradesGetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderTradesResponseData(code: 404, message: "Not Found"));

        // Act
        var exception = await Assert.ThrowsAsync<ItApiException>(async () =>
            await _itService.GetTradeDetail(requestModel, CancellationToken.None));

        // Assert
        Assert.Equal($"ItApiException GetTradeDetail: Error while calling IT Api for AccountNo: {requestModel.AccountNo}", exception.Message);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public async void GetTradeDetail_ShouldReturnDataCorrectly(int page, int pageSize)
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            page,
            pageSize,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"));

        _backOfficeApi.Setup(s => s.ApiBackOfficeOrderTradesGetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderTradesResponseData(code: 0, message: "success", data: MockTradeDetail()));

        // Act
        var result = await _itService.GetTradeDetail(requestModel, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TradeDetails.Count > 0);
    }

    [Theory]
    [InlineData(5, 1)]
    public async void GetTradeDetail_ShouldReturnEmpty(int page, int pageSize)
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            page,
            pageSize,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"));

        _backOfficeApi.Setup(s => s.ApiBackOfficeOrderTradesGetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderTradesResponseData(code: 0, message: "success", data: MockTradeDetail()));

        // Act
        var result = await _itService.GetTradeDetail(requestModel, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TradeDetails.Count == 0);
    }

    [Fact]
    public async void GetTfexData_ShouldReturnDataCorrectly()
    {
        // Arrange
        var response = new PositionTFEXResponseData(
            1, "message", 1, DateTime.Now, new PositionTFEX(
                "", "", "", "", "", "", "", 100, 200, 300, 10, 10,
                [new PositionItem("", 1, 0, 10, 1, 0, 35, 0, "THB", 1, 1)]
            )
        );
        _backOfficeApi.Setup(s => s.ApiBackOfficePositionTFEXGetAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _itService.GetTfexData("1234567", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response.Code, result.Code);
        Assert.Equal(response.Data.Account, result.Data.Account);
        Assert.Equal((decimal?)response.Data.Equity, result.Data.Equity);
    }

    [Fact]
    public async void GetTfexData_ShouldThrowExceptionWhenResponseIsNotSuccess()
    {
        // Arrange
        const string custCode = "1234567";

        // Act
        var exception = await Assert.ThrowsAsync<ItApiException>(async () =>
            await _itService.GetTfexData(custCode, CancellationToken.None));

        // Assert
        Assert.Equal($"ItApiException GetTfexData: Error while calling IT Api for AccountNo: {custCode}", exception.Message);
    }

    private static List<TradeDetail> MockTradeDetail()
    {
        return
        [
            new TradeDetail(shareCode: "AAAZ24",
                refType: "BU",
                buysellsorter: "C",
                price: 12.34,
                unit: 5,
                amt: 10,
                commSub: 1,
                vatSub: 2,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 10, minutes: 5, seconds: 10)
            ),
            new TradeDetail(shareCode: "BBBZ24",
                refType: "BH",
                buysellsorter: "O",
                price: 23.45,
                unit: 6,
                amt: 11,
                commSub: 1.5,
                vatSub: 2.5,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 11, minutes: 10, seconds: 50)
            ),
            new TradeDetail(shareCode: "CCCZ24",
                refType: "SE",
                buysellsorter: "C",
                price: 85.44,
                unit: 8,
                amt: 90,
                commSub: 2,
                vatSub: 3,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 4, minutes: 6, seconds: 10)
            ),
            new TradeDetail(shareCode: "CCCZ24",
                refType: "SH",
                buysellsorter: "O",
                price: 55.42,
                unit: 1,
                amt: 1,
                commSub: 0.1,
                vatSub: 0.1,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 5, minutes: 7, seconds: 10)
            )
        ];
    }
}