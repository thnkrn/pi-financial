using System.ComponentModel;
using System.Reflection;
using Pi.Common.ExtensionMethods;
using Pi.FundMarketData.API.Attributes;
using Pi.FundMarketData.API.Models.Filters;
using Pi.FundMarketData.API.Models.Requests;
using Pi.FundMarketData.API.Models.Responses;
using Pi.FundMarketData.Filters;

namespace Pi.FundMarketData.API.Factories;

public static class FilterFactory
{
    public static FundFilter NewFundFilter(BasketFiltersRequest filtersRequest)
    {
        string[] categories = filtersRequest?.FundTypes?.Contains(FundType.All) == true ? null : filtersRequest?.FundTypes?.Select(q => q.GetEnumDescription()).ToArray();
        return new FundFilter
        {
            Categories = categories,
            TaxTypes = filtersRequest?.TaxSavingTypes?.Select(q => q.ToString()).ToArray(),
            AmcCodes = filtersRequest?.AmcCodes?.Select(q => q.ToString()).ToArray(),
            RiskLevels = filtersRequest?.RiskLevels?.Select(q => (int)q).ToArray(),
            Dividend = filtersRequest?.Dividend switch
            {
                Dividend.Yes => true,
                Dividend.No => false,
                _ => null
            }
        };
    }

    public static FundFilter NewFundFilter(BasketFiltersRequestV2 filtersRequest)
    {
        string[] categories = filtersRequest?.FundTypes?.Contains(FundType.All) == true ? null : filtersRequest?.FundTypes?.Select(q => q.GetEnumDescription()).ToArray();
        return new FundFilter
        {
            Categories = categories,
            TaxTypes = filtersRequest?.TaxSavingTypes?.Select(q => q.ToString()).ToArray(),
            AmcCodes = filtersRequest?.AmcCodes?.Select(q => q.ToString()).ToArray(),
            RiskLevels = filtersRequest?.RiskLevels?.Select(q => (int)q).ToArray(),
            Dividend = filtersRequest?.Dividend switch
            {
                DividendV2.Dividend => true,
                DividendV2.NotDividend => false,
                _ => null
            }
        };
    }

    public static FilterResponse[] NewFilterResponse<T>() where T : IBasketFilterRequest
    {
        return typeof(T).GetProperties().Select(q =>
        {
            FilterOption[] options = [];
            var optionIndex = 0;

            // Get enum type from array enum
            if (q.PropertyType.IsArray && q.PropertyType.GetElementType() is { IsEnum: true })
            {
                options = q.PropertyType.GetElementType()!.GetFields(BindingFlags.Public | BindingFlags.Static).Select(x =>
                {
                    optionIndex += 1;
                    return NewFilterOption(x, optionIndex);
                }).ToArray();
            }
            else
            {
                // Get enum type from enum nullable
                var nullableType = Nullable.GetUnderlyingType(q.PropertyType);
                if (nullableType != null)
                {
                    options = nullableType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(x =>
                    {
                        optionIndex += 1;
                        return NewFilterOption(x, optionIndex);
                    }).ToArray();
                }
            }

            return new FilterResponse
            {
                FilterCategory = q.GetCustomAttribute<DescriptionAttribute>()?.Description ?? q.Name,
                FilterKey = q.Name,
                FilterType = q.GetCustomAttribute<FilterTypeAttribute>()?.FilterType ?? FilterType.Secondary,
                Filters = options.ToArray(),
                Order = q.GetCustomAttribute<FilterOrderAttribute>()?.Order ?? 0
            };
        }).ToArray();
    }

    private static FilterOption NewFilterOption(FieldInfo fieldInfo, int index = 0)
    {
        return new FilterOption
        {
            FilterName = fieldInfo.GetCustomAttribute<DescriptionAttribute>()!.Description,
            Value = fieldInfo.Name,
            IsDefault = fieldInfo.GetCustomAttribute<FilterDefaultAttribute>() != null,
            Order = index
        };
    }
}
