using Pi.SetMarketData.Application.Interfaces.BrokerIdMapperService;
using Pi.SetMarketData.Domain.Entities.SetSmart;

namespace Pi.SetMarketData.Application.Services.BrokerId;

public class BrokerIdMapperService : IBrokerIdMapperService
{
    public IDictionary<string, string> GetBrokerIdMap(IEnumerable<BrokerInfo> brokers)
    {
        var brokerIdMap = new Dictionary<string, string>();
        foreach (var broker in brokers)
        {
            if (!string.IsNullOrEmpty(broker.BrokerCode) && !string.IsNullOrEmpty(broker.BrokerId))
            {
                brokerIdMap[broker.BrokerCode] = broker.BrokerId;
            }
        }
        return brokerIdMap;
    }
}