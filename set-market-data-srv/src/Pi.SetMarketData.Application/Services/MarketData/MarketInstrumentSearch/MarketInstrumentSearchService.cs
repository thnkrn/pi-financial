using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketInstrumentSearch
{
    public static class MarketInstrumentSearchService
    {
        public static MarketInstrumentSearchResponse GetResult(
            List<Instrument> instruments,
            List<MarketStreamingResponse> marketStreamingList,
            List<InstrumentDetail> instrumentDetailsList)
        {
            var groupedInstruments = instruments
                .GroupBy(ins => new { ins.InstrumentType, ins.InstrumentCategory })
                .ToList();

            var instrumentCategoryList = new List<InstrumentSearchCategoryList>();

            for (int i = 0; i < groupedInstruments.Count; i++)
            {
                var currentGroup = groupedInstruments[i];
                var instrumentSearchList = new List<InstrumentSearchList>();

                for (int j = 0; j < instruments.Count; j++)
                {
                    var currentInstrument = instruments[j];

                    if (currentGroup.Key.InstrumentType == currentInstrument.InstrumentType &&
                        currentGroup.Key.InstrumentCategory == currentInstrument.InstrumentCategory)
                    {
                        var marketStreaming = marketStreamingList[j]?.Response?.Data?.FirstOrDefault();
                        var instrumentDetail = instrumentDetailsList[j];

                        instrumentSearchList.Add(new InstrumentSearchList
                        {
                            Venue = currentInstrument.Venue,
                            Symbol = currentInstrument.Symbol,
                            FriendlyName = string.IsNullOrEmpty(currentInstrument.FriendlyName) ?
                                currentInstrument.LongName :
                                currentInstrument.FriendlyName,
                            Logo = currentInstrument.Logo,
                            Price = DataManipulation.FormatDecimals(
                                marketStreaming?.Price ?? "",
                                instrumentDetail.DecimalsInPrice),
                            PriceChange = DataManipulation.FormatDecimals(
                                marketStreaming?.PriceChanged ?? "",
                                instrumentDetail.DecimalsInPrice),
                            PriceChangeRatio = DataManipulation.FormatDecimals(
                                marketStreaming?.PriceChangedRate ?? "",
                                instrumentDetail.DecimalsInPrice),
                            IsFavorite = false,
                            Unit = string.Empty
                        });
                    }
                }

                instrumentCategoryList.Add(new InstrumentSearchCategoryList
                {
                    Order = i + 1, // TBC
                    InstrumentType = currentGroup.Key.InstrumentType,
                    InstrumentCategory = currentGroup.Key.InstrumentCategory,
                    InstrumentList = instrumentSearchList
                });
            }

            return new MarketInstrumentSearchResponse
            {
                Code = "0",
                Message = string.Empty,
                Response = new InstrumentSearchResponse
                {
                    InstrumentCategoryList = instrumentCategoryList
                }
            };
        }
    }
}

