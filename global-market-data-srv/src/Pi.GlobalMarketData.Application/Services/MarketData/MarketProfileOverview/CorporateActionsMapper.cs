
namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileOverview;
public class CorporateActionsMapper
{
    public List<Domain.Entities.CorporateAction> MapCorporateAction(List<Domain.Entities.CorporateAction> data)
    {
        var corporateActions = new List<Domain.Entities.CorporateAction>();
        foreach (var item in data)
        {
            corporateActions.Add(new Domain.Entities.CorporateAction
            {
                CorporateActionId = item.CorporateActionId,
                InstrumentId = item.InstrumentId,
                Code = item.Code,
                Date = item.Date,

            });
        }
        return corporateActions;
    }
}