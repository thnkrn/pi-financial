using Pi.SetService.Application.Filters;
using Pi.SetService.Application.Models;

namespace Pi.SetService.Application.Factories;

public static class FilterFactory
{
    public static SblOrderFilters NewSblOrderFilters(SetOrderFilters filters)
    {
        bool? openFilter = filters.HistoryOrder == true ? false : null;

        return new SblOrderFilters()
        {
            Open = filters.OpenOrder == true ? true : openFilter,
            CreatedDateFrom = filters.EffectiveDateFrom,
            CreatedDateTo = filters.EffectiveDateTo,
        };
    }
}
