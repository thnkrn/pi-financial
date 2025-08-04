using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Application.Tests;

public static class FakeFactory
{
    public static FundInfo NewFundInfo(string fundCode, string amcCode = "amc", string? taxType = default)
    {
        return new FundInfo("กองทุนเปิด ทิสโก้ Next Generation Internet ชนิดผู้ลงทุนทั่วไป", fundCode, "some url", amcCode)
        {
            Nav = 10,
            InstrumentCategory = "Funds",
            FirstMinBuyAmount = 1000,
            NextMinBuyAmount = 1000,
            MinSellAmount = 1000,
            MinSellUnit = 1000,
            MinBalanceAmount = 100,
            MinBalanceUnit = 10,
            TaxType = taxType,
            PiBuyCutOffTime = DateTime.UtcNow.AddHours(2),
            PiSellCutOffTime = DateTime.UtcNow.AddHours(2)
        };
    }
}
