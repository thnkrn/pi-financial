using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Services.ItchMapper;

public class ItchPriceInfoMapperService : IItchPriceInfoMapperService
{
    public PriceInfo? Map(ItchMessage? message)
    {
        if (message == null)
            return null;

        var priceInfo = new PriceInfo();

        switch (message.MsgType)
        {
            case ItchMessageType.i:
                {
                    // Map from MsgType : i
                    var wrapper = (TradeTickerMessageWrapper)message;
                    priceInfo.OrderBookId = wrapper.OrderbookId.Value.ToString();
                    priceInfo.Price = wrapper.Price.Value.ToString();
                    priceInfo.Volume = wrapper.Quantity.Value.ToString();
                    priceInfo.Aggressor = wrapper.Aggressor.Value;
                    priceInfo.Amount = (wrapper.Price.Value * wrapper.Quantity.Value).ToString();

                    switch (priceInfo.Aggressor)
                    {
                        case "S":
                            priceInfo.AverageSell = wrapper.Price.Value.ToString();
                            break;
                        case "B":
                            priceInfo.AverageBuy = wrapper.Price.Value.ToString();
                            break;
                    }
                    break;
                }
            case ItchMessageType.I:
                {
                    // Map from MsgType : I
                    var wrapper = (TradeStatisticsMessageWrapper)message;
                    priceInfo.AuctionPrice = wrapper.LastAuctionPrice.Value.ToString();
                    priceInfo.AuctionVolume = wrapper.TurnoverQuantity.Value.ToString();
                    priceInfo.Open = wrapper.OpenPrice.Value.ToString();
                    priceInfo.High24H = wrapper.HighPrice.Value.ToString();
                    priceInfo.Low24H = wrapper.LowPrice.Value.ToString();
                    priceInfo.Average = wrapper.AveragePrice.Value.ToString();
                    priceInfo.TotalAmount = wrapper.TurnoverValue.Value.ToString();
                    priceInfo.TotalAmountK = wrapper.TurnoverValue.Value.ToString();
                    priceInfo.TotalVolume = wrapper.TurnoverQuantity.Value.ToString();
                    priceInfo.TotalVolumeK = wrapper.TurnoverQuantity.Value.ToString();
                    break;
                }
            case ItchMessageType.Q:
                {
                    // Map from MsgType : Q
                    var wrapper = (ReferencePriceMessageWrapper)message;
                    priceInfo.PriceChanged = wrapper.Price.Value.ToString();
                    priceInfo.PreClose = wrapper.Price.Value.ToString();
                    break;
                }
            case ItchMessageType.R:
                {
                    // Map from MsgType : R
                    var wrapper = (OrderBookDirectoryMessageWrapper)message;
                    priceInfo.Symbol = wrapper.Symbol.Value;
                    priceInfo.LastTrade = wrapper.LastTradingDate.ToString();
                    priceInfo.Currency = wrapper.TradingCurrency.Value;
                    break;
                }
            case ItchMessageType.J:
                {
                    // Map from MsgType : J
                    var indexPriceMessage = (IndexPriceMessageWrapper)message;

                    priceInfo.Price = indexPriceMessage.Value.Value.ToString();
                    priceInfo.Open = indexPriceMessage.OpenValue.Value.ToString();
                    priceInfo.High24H = indexPriceMessage.HighValue.Value.ToString();
                    priceInfo.Low24H = indexPriceMessage.LowValue.Value.ToString();
                    priceInfo.PriceChanged = indexPriceMessage.Change.Value.ToString();
                    priceInfo.PriceChangedRate = indexPriceMessage.ChangePercent.Value.ToString();
                    priceInfo.PreClose = indexPriceMessage.PreviousClose.Value.ToString();
                    priceInfo.TotalAmount = indexPriceMessage.TradedValue.Value.ToString();
                    priceInfo.TotalVolume = indexPriceMessage.TradedVolume.Value.ToString();
                    priceInfo.TotalAmountK = ((ulong)indexPriceMessage.TradedValue.Value / 100).ToString();
                    priceInfo.TotalVolumeK = ((ulong)indexPriceMessage.TradedVolume.Value / 100).ToString();
                    break;
                }
        }

        return priceInfo;
    }
}