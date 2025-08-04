using Pi.SetMarketDataWSS.Application.Interfaces;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.OrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Domain.Entities;
using Newtonsoft.Json;

namespace Pi.SetMarketDataWSS.Application.Tests.Services.ItchMapper;

public class ItchMapperServiceTest
{
    private readonly Mock<IItchPriceInfoMapperService> _priceinfoMapperService;
    private readonly Mock<IOrderBookMapper> _orderBookMapperService;
    private readonly Mock<IItchPublicTradeMapper> _publicTradeMapperService;
    private readonly Mock<IItchOpenInterestMapperService> _openInterestMapperService;
    private readonly Mock<IItchTickSizeTableEntryMapperService> _tickSizeTableEntryMapperService;

    ItchMapperService mapperService;
    public ItchMapperServiceTest
    ()
    {
        _priceinfoMapperService = new Mock<IItchPriceInfoMapperService>();
        _orderBookMapperService = new Mock<IOrderBookMapper>();
        _publicTradeMapperService = new Mock<IItchPublicTradeMapper>();
        _openInterestMapperService = new Mock<IItchOpenInterestMapperService>();
        _tickSizeTableEntryMapperService = new Mock<IItchTickSizeTableEntryMapperService>();
        

        mapperService = new ItchMapperService(_priceinfoMapperService.Object, _publicTradeMapperService.Object, _orderBookMapperService.Object, _tickSizeTableEntryMapperService.Object, _openInterestMapperService.Object);    
    }

    [Fact]
    public void MapToDataCache_ReturnRedisValueWithUnderlyingPrice_WhenUnderlyingOrderBookIDIsNotNull()
    {
        // Arrange
        OrderBookDirectoryMessageWrapper msg = new OrderBookDirectoryMessageWrapper{
            OrderBookID = new OrderBookId{ Value = 7771},
            UnderlyingOrderBookID = new UnderlyingOrderBookId{ Value = 777}
        };
        PriceInfo underlyingPrice = new PriceInfo {
            Price = "250.00"
        };
        var strUnderlying = JsonConvert.SerializeObject(underlyingPrice);
        Dictionary<string, string> cachedValue = new Dictionary<string, string>
        {
            {
                $"{CacheKey.PriceInfo}{msg.UnderlyingOrderBookID}",
                strUnderlying
            }
        };


        // Act
        var redisValueResult = mapperService.MapToDataCache(msg, cachedValue);

        // Assert
        var redisValue = redisValueResult?.RedisValue;
        InstrumentDetail instrumentDetail = (InstrumentDetail)redisValue[^1].Value;
        Assert.Equal(underlyingPrice.Price, instrumentDetail?.UnderlyingPrice);
    }
    [Fact]
    public void MapToDataCache_ReturnRedisValueWithoutZeroUnderlyingPrice_WhenUnderlyingOrderBookIDIsNull()
    {
        // Arrange
        OrderBookDirectoryMessageWrapper msg = new OrderBookDirectoryMessageWrapper{
            OrderBookID = new OrderBookId{ Value = 7771},
            UnderlyingOrderBookID = new UnderlyingOrderBookId{ Value = 777}
        };
        
        Dictionary<string, string> cachedValue = new Dictionary<string, string>
        {
            {
                $"{CacheKey.PriceInfo}{msg.UnderlyingOrderBookID}",
                string.Empty
            }
        };


        // Act
        var redisValueResult = mapperService.MapToDataCache(msg, cachedValue);

        // Assert
        var redisValue = redisValueResult?.RedisValue;
        InstrumentDetail instrumentDetail = (InstrumentDetail)redisValue[^1].Value;
        Assert.Equal("0.00", instrumentDetail?.UnderlyingPrice);
    }
}