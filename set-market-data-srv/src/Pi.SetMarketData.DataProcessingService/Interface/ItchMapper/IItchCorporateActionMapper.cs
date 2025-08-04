using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.DataProcessingService.Interface.ItchMapper;

public interface IItchCorporateActionMapper
{
    CorporateAction MapCorporateAction(List<CorporateAction> data);
}