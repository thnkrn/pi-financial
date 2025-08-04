using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Services.ItchMapper;
using AutoFixture;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Domain.Models.DataProcessing;
using MongoDB.Bson;

namespace Pi.SetMarketData.Application.Tests.Services.ItchMapper;

public class ItchMapperServiceTest
{
    IItchMapperService itchMapperService;

    Mock<IItchPriceInfoMapperService> _itchPriceInfoMapperService;
    Mock<IItchOrderBookMapperService> _itchOrderBookMapperService;
    Mock<IItchOrderBookDirectoryMapperService> _itchOrderBookDirectoryMapperService;
    Mock<IItchTickSizeTableEntryMapperService> _itchTickSizeTableMapperSerice;

    public ItchMapperServiceTest()
    {
        _itchPriceInfoMapperService = new Mock<IItchPriceInfoMapperService>();
        _itchOrderBookMapperService = new Mock<IItchOrderBookMapperService>();
        _itchOrderBookDirectoryMapperService = new Mock<IItchOrderBookDirectoryMapperService>();
        _itchTickSizeTableMapperSerice = new Mock<IItchTickSizeTableEntryMapperService>();

        // Initiate itchMapperService instance
        itchMapperService = new ItchMapperService(
            _itchPriceInfoMapperService.Object,
            _itchOrderBookMapperService.Object, 
            _itchOrderBookDirectoryMapperService.Object,
            _itchTickSizeTableMapperSerice.Object
        );
    }

    [Fact]
    public void MapToDatabase_Return_DataProcessingResult_MSG_i()
    {
        // Arrange\
        var fixture = new Fixture();
        fixture.Register(ObjectId.GenerateNewId);
        var message = fixture.Create<Mock<TradeTickerMessageWrapper>>().Object;

        var priceInfo = fixture.Create<Mock<PriceInfo>>().Object;
        priceInfo.Price = fixture.Create<Price>().Value.ToString();

        _itchPriceInfoMapperService.Setup(x => x.Map(It.IsAny<TradeTickerMessageWrapper>())).Returns(priceInfo);

        // Act
        var mappedResult = itchMapperService.MapToDatabase(message, null, null, null, null).FirstOrDefault();
        var convertedResult = (PriceInfo)mappedResult.Values[0].Value;

        // Assert
        Assert.IsType<DataProcessingResult>(mappedResult);
        Assert.Equal(priceInfo.Price, convertedResult?.Price);
    }

    [Theory]
    [InlineData("XA,XD", "XA,XD")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void MapToCorporateAction_ShouldReturnCorrectCorporateAction(string code, string expected)
    {
        // Arrange
        var message = new OrderBookDirectoryMessageWrapper
        {
            CorporateActionCode = new CorporateActionCode { Value = code },
            OrderBookID = new OrderBookID { Value = 0 }
        };

        var _itchOrderBookDirectoryMapperService = new ItchOrderBookDirectoryService();

        // Act
        var corporateAction = _itchOrderBookDirectoryMapperService.MapToCorporateAction(message);

        // Assert
        Assert.Equal(expected, corporateAction?.Code);
    }

    [Theory]
    [InlineData("NC,X", "T", "NC,X,T")]
    [InlineData("NC", "T", "NC,T")]
    [InlineData("", "O", "O")]
    [InlineData("N", "", "N")]
    [InlineData("", "", null)]
    [InlineData(null, null, null)]
    public void MapToTradingSign_ShouldReturnCorrectTradingSign(string notificationSignValue, string otherSignValue, string expected)
    {
        // Arrange
        var message = new OrderBookDirectoryMessageWrapper
        {
            NotificationSign = new NotificationSign { Value = notificationSignValue },
            OtherSign = new OtherSign { Value = otherSignValue },
            OrderBookID = new OrderBookID { Value = 0 }
        };

        var _itchOrderBookDirectoryMapperService = new ItchOrderBookDirectoryService();

        // Act
        var tradingSigns = _itchOrderBookDirectoryMapperService.MapToTradingSign(message);

        // Assert
        Assert.Equal(expected, tradingSigns?.Sign);
    }

}