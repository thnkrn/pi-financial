
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.DataProcessingService.Interface.ItchMapper
{
    public interface IItchCorporateActionMapper
    {
        List<Domain.Entities.ProfileOverview.CorporateActionResponse> MapCorporateAction(CorporateAction data);
    }
}
