using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketFilters
{
    public static class MarketFiltersService
    {
        public static MarketFiltersResponse GetResult(List<CuratedFilter> curatedFilters)
        {
            var _data = new List<Body>();

            if (curatedFilters == null || curatedFilters.Count == 0)
            {
                return new MarketFiltersResponse
                {
                    Code = "0",
                    Message = "",
                    Response = new FiltersResponse { Data = _data }
                };
            }

            var groupedFilters = curatedFilters
                .GroupBy(cf => new
                {
                    cf.FilterCategory,
                    cf.FilterType,
                    cf.CategoryPriority
                })
                .ToList();

            foreach (var group in groupedFilters)
            {
                var filterList = new List<Domain.Models.Response.Filter>();

                foreach (var cf in group)
                {
                    filterList.Add(
                        new Domain.Models.Response.Filter
                        {
                            FilterId = cf.FilterId,
                            FilterName = cf.FilterName,
                            IsHighLight = Convert.ToBoolean(cf.Highlight),
                            IsDefault = Convert.ToBoolean(cf.IsDefault),
                            Order = cf.Ordering
                        }
                    );
                }

                _data.Add(
                    new Body
                    {
                        FilterType = group.Key.FilterType,
                        FilterCategory = group.Key.FilterCategory,
                        SupportSecondaryFilter = group.Key?.FilterType?.ToLower() == "primary",
                        Order = group.Key.CategoryPriority,
                        Filters = filterList.OrderBy(t => t.Order).ToList()
                    }
                );
            }

            return new MarketFiltersResponse
            {
                Code = "0",
                Message = "",
                Response = new FiltersResponse { Data = _data }
            };
        }
    }
}
