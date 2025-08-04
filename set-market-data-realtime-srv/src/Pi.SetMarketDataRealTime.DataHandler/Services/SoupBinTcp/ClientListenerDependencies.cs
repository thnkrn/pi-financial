using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public class ClientListenerDependencies(
    IItchParserService itchParserService,
    IWriteBinLogsData writeBinLogsData,
    IStockDataOptimizedV2Publisher kafkaPublisher,
    IMemoryCacheHelper memoryCacheHelper,
    IDisconnectionHandlerFactory disconnectionHandlerFactory)
{
    public IDisconnectionHandlerFactory DisconnectionHandlerFactory { get; } = disconnectionHandlerFactory;
    public IItchParserService ItchParserService { get; } = itchParserService;
    public IStockDataOptimizedV2Publisher KafkaPublisher { get; } = kafkaPublisher;
    public IMemoryCacheHelper MemoryCacheHelper { get; } = memoryCacheHelper;
    public IWriteBinLogsData WriteBinLogsData { get; } = writeBinLogsData;
}