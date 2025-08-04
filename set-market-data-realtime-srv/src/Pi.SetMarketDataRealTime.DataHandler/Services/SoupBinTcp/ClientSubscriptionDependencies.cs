using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public class ClientSubscriptionDependencies(
    IClientFactory clientFactory,
    IClientListener clientListener,
    IMessageValidator messageValidator,
    IMemoryCacheHelper memoryCacheHelper,
    IAmazonS3Service s3Service)
{
    public IClientFactory ClientFactory { get; } = clientFactory;
    public IClientListener ClientListener { get; } = clientListener;
    public IMessageValidator MessageValidator { get; } = messageValidator;
    public IMemoryCacheHelper MemoryCacheHelper { get; } = memoryCacheHelper;
    public IAmazonS3Service S3Service { get; } = s3Service;
}