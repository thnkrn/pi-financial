using System.Collections.Concurrent;
using Pi.SetMarketData.Application.Interfaces.MarketFilterInstruments;
using Pi.SetMarketData.Application.Queries.MarketFilterInstruments;
using Pi.SetMarketData.Application.Services.MarketData.MarketFilterInstruments;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketFilterInstruments
{
    public class PostMarketFilterInstrumentsRequestHandler : PostMarketFilterInstrumentsAbstractHandler
    {
        private readonly IMongoService<CuratedFilter> _curatedFilterService;
        private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
        private readonly ICacheService _cacheService;
        private readonly IEntityCacheService _entityCacheService;
        private readonly IInstrumentHelper _instrumentHelper;
        private readonly ITimescaleService<RealtimeMarketData> _realtimeMarketDataService;

        public PostMarketFilterInstrumentsRequestHandler
        (
            IMongoService<CuratedFilter> curatedFilterService,
            IMongoService<Domain.Entities.Instrument> instrumentService,
            ICacheService cacheService,
            IEntityCacheService entityCacheService,
            IInstrumentHelper instrumentHelper,
            ITimescaleService<RealtimeMarketData> realtimeMarketDataService
        )
        {
            _curatedFilterService = curatedFilterService;
            _instrumentService = instrumentService;
            _cacheService = cacheService;
            _entityCacheService = entityCacheService;
            _instrumentHelper = instrumentHelper;
            _realtimeMarketDataService = realtimeMarketDataService;
        }

        protected override async Task<PostMarketFilterInstrumentsResponse> Handle(
            PostMarketFilterInstrumentsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var curatedFiltersList = new List<CuratedFilter>();
                var instruments = new List<Domain.Entities.Instrument>();

                foreach (var filterId in request.Data.FilterList ?? [])
                {
                    var curatedFilter = await _curatedFilterService.GetByFilterAsync(target =>
                        target.FilterId == filterId &&
                        target.GroupName == request.Data.GroupName &&
                        target.SubGroupName == request.Data.SubGroupName
                    );
                    if (curatedFilter == null) continue;
                    curatedFiltersList.Add(curatedFilter);

                    var instrument = await _entityCacheService.GetInstrument(curatedFilter);
                    instruments.AddRange(instrument);
                }

                // Update FriendlyName
                instruments = (await Task.WhenAll(
                    instruments.AsParallel().AsOrdered().Select(async instrument =>
                    {
                        if (!string.IsNullOrEmpty(instrument.FriendlyName)) return instrument;
                        instrument.FriendlyName = await _instrumentHelper.GetFriendlyName(
                            instrument.Symbol!,
                            instrument.InstrumentCategory ?? string.Empty
                        );
                        return instrument;
                    })
                )).ToList();

                var instrumentOrderBookIdMap = new ConcurrentDictionary<Domain.Entities.Instrument, int?>();
                await Task.WhenAll(instruments.Select(async instrument =>
                {
                    var instrumentDetail = await _cacheService.GetAsync<Domain.Entities.InstrumentDetail>
                        ($"{CacheKey.InstrumentDetail}{instrument.OrderBookId}");

                    if (instrumentDetail?.UnderlyingOrderBookID != null ||
                        instrumentDetail?.UnderlyingOrderBookID != 0
                    ) instrumentOrderBookIdMap[instrument] = instrumentDetail?.UnderlyingOrderBookID;
                    else instrumentOrderBookIdMap[instrument] = null;
                }));

                var orderBookId = new HashSet<int?>(instrumentOrderBookIdMap.Values);
                orderBookId.Remove(null);
                var underlyingInstruments = await _instrumentService.GetAllByFilterAsync(target =>
                    orderBookId.Contains(target.OrderBookId) && target.Deprecated != true
                );

                var orderedInstruments = instruments.ToList();
                var matchedUnderlyingInstruments = new List<(Domain.Entities.Instrument Instrument, Domain.Entities.Instrument? UnderlyingInstrument)>();
                foreach (var instrument in orderedInstruments)
                {
                    var underlyingOrderBookId = instrumentOrderBookIdMap.GetValueOrDefault(instrument);
                    var underlyingInstrument = underlyingInstruments.FirstOrDefault(ui => ui.OrderBookId == underlyingOrderBookId);
                    matchedUnderlyingInstruments.Add((instrument, underlyingInstrument));
                }

                var logos = GetLogos(
                    matchedUnderlyingInstruments.Select(x => x.Instrument),
                    matchedUnderlyingInstruments.Select(x => x.UnderlyingInstrument)
                );

                var cacheKeys = instruments
                    .Where(instrument =>
                        instrument?.OrderBookId != null
                        && instrument?.OrderBookId != 0
                    )
                    .Select(instrument => $"{CacheKey.SetStreamingBody}{instrument.OrderBookId}")
                    .ToList();

                var priceResponseResult = await _cacheService.GetBatchAsync<PriceResponse>(cacheKeys);

                var priceResponses = cacheKeys.Select(key =>
                    priceResponseResult.TryGetValue(key, out var priceResponse)
                        ? priceResponse
                        : null
                ).ToList();

                var result = MarketFilterInstrumentsService.GetResult
                (
                    curatedFiltersList,
                    instruments.ToList(),
                    priceResponses,
                    logos.ToList()
                );

                return new PostMarketFilterInstrumentsResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static IEnumerable<string> GetLogos
        (
            IEnumerable<Domain.Entities.Instrument> instruments,
            IEnumerable<Domain.Entities.Instrument?> underlyingInstruments
        ) => instruments.Zip(underlyingInstruments, (instrument, underlyingInstrument) =>
        {
            string market;
            if (underlyingInstrument != null)
            {
                market = string.IsNullOrEmpty(underlyingInstrument.Venue) ? "SET" : underlyingInstrument.Venue;
                return LogoHelper.GetLogoUrl(market, underlyingInstrument.SecurityType ?? string.Empty, underlyingInstrument.Symbol ?? string.Empty);
            }

            market = string.IsNullOrEmpty(instrument.Venue) ? "SET" : instrument.Venue;
            return LogoHelper.GetLogoUrl(market, instrument.SecurityType ?? string.Empty, instrument.Symbol ?? string.Empty);
        });
    }
}
