using Pi.SetMarketData.Domain.Entities.SetSmart;

namespace Pi.SetMarketData.Application.Interfaces.BrokerIdMapperService;

public interface IBrokerIdMapperService
{
    IDictionary<string, string> GetBrokerIdMap(IEnumerable<BrokerInfo> brokers);
}