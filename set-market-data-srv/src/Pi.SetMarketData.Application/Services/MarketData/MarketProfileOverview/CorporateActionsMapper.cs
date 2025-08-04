using Pi.SetMarketData.DataProcessingService.Interface.ItchMapper;

namespace Pi.SetMarketData.Application.Interfaces.MarketProfileOverview;
public class CorporateActionsMapper : IItchCorporateActionMapper
{
    public List<Domain.Entities.ProfileOverview.CorporateActionResponse> MapCorporateAction(Domain.Entities.CorporateAction data)
    {
        var corporateActions = new List<Domain.Entities.ProfileOverview.CorporateActionResponse>();
        if (string.IsNullOrEmpty(data?.Code))
            return new List<Domain.Entities.ProfileOverview.CorporateActionResponse>
                {
                    new Domain.Entities.ProfileOverview.CorporateActionResponse
                    {
                        Date = string.Empty,
                        Type = string.Empty,
                    }
                };

        string[] codes = data.Code?.Split(',') ?? [];
        if (codes.Length == 0)
        {
            return new List<Domain.Entities.ProfileOverview.CorporateActionResponse>
            {
                new Domain.Entities.ProfileOverview.CorporateActionResponse
                {
                    Date = data.Date ?? string.Empty,
                    Type = data.Code ?? string.Empty,
                }
            };
        }

        foreach (var code in codes)
        {
            corporateActions.Add(new Domain.Entities.ProfileOverview.CorporateActionResponse
            {
                Date = data.Date ?? string.Empty,
                Type = code ?? string.Empty,
            });
        }
        return corporateActions;
    }
}