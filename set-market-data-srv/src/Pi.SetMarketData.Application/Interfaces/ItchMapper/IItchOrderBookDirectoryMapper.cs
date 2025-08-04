using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Interfaces.ItchMapper
{
    public interface IItchOrderBookDirectoryMapperService
    {
        public Domain.Entities.Instrument? MapToInstrument(ItchMessage? message, MarketDirectory? marketDirectory);
        public Domain.Entities.InstrumentDetail? MapToInstrumentDetail(ItchMessage? message);
        public Domain.Entities.OrderBook? MapToOrderBook(ItchMessage? message);
        public Domain.Entities.WhiteList? MapToWhiteList(ItchMessage? message, bool? exchangeServer);
        public Domain.Entities.CorporateAction? MapToCorporateAction(ItchMessage? message);
        public Domain.Entities.TradingSign? MapToTradingSign(ItchMessage? message);
    }
}