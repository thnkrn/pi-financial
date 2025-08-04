using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Utils;

public static class UnitHolderUtils
{
    public static UnitHolderType GetUnitHolderType(string[] segTaxTypes, FundInfo fundInfo)
    {
        return segTaxTypes.Contains(fundInfo.TaxType) ? UnitHolderType.SEG : UnitHolderType.OMN;
    }

    public static PaymentType GetPaymentTypeFromUnitHolderType(UnitHolderType unitHolderType)
    {
        return unitHolderType == UnitHolderType.OMN ? PaymentType.AtsSa : PaymentType.AtsAmc;
    }

    public static CustomerAccountUnitHolder? GetUnitholder(List<CustomerAccountUnitHolder> unitHolders, UnitHolderType fundUnitHolderType)
    {
        CustomerAccountUnitHolder? unitHolder = null;
        switch (unitHolders.Count)
        {
            case 1:
                if (fundUnitHolderType == unitHolders.First().UnitHolderType)
                {
                    unitHolder = unitHolders.First();
                }
                break;
            case > 1:
                {
                    unitHolders.Sort((a, b) => string.Compare(a.UnitHolderId, b.UnitHolderId, StringComparison.Ordinal));
                    unitHolder = unitHolders.FirstOrDefault(q => q.UnitHolderType == fundUnitHolderType);
                    break;
                }
        }

        return unitHolder;
    }
}
