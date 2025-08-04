using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Domain.Tests.AggregatesModel.FinancialAssetAggregate;

public class FundInfoTest
{
    [Theory]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.UltraHighNetWorth, true)]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.HighNetWorth, true)]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.Retail, false)]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.Institutional, false)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.UltraHighNetWorth, true)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.HighNetWorth, true)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.Retail, false)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.Institutional, false)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.UltraHighNetWorth, true)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.HighNetWorth, true)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.Retail, false)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.Institutional, false)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.UltraHighNetWorth, true)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.HighNetWorth, false)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.Retail, false)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.Institutional, false)]
    public void Should_Return_Expected_When_VerifyCustomerAccess(FundProjectType projectType, InvestorClass investorClass, bool expected)
    {
        // Arrange
        var fundInfo = NewFundInfo(projectType);
        var customer = new CustomerAccountDetail
        {
            IcLicense = "0851",
            InvestorClass = investorClass,
            CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder>()
        };

        // Act
        var actual = fundInfo.VerifyCustomerAccess(customer);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(FundProjectType.NonRetailInvestorsFund, false)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, false)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, false)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, false)]
    [InlineData(FundProjectType.Unknown, true)]
    [InlineData(FundProjectType.GeneralInvestorsFund, true)]
    [InlineData(FundProjectType.InstitutionalInvestorsFund, true)]
    [InlineData(FundProjectType.CorporateInvestorsFund, true)]
    [InlineData(FundProjectType.ThaiGovernmentFund, true)]
    [InlineData(FundProjectType.PensionReserveFund, true)]
    public void Should_Return_Expected_When_VerifyCustomerAccess_And_InvestorClass_Is_Null(FundProjectType projectType,
        bool expected)
    {
        // Arrange
        var fundInfo = NewFundInfo(projectType);
        var customer = new CustomerAccountDetail
        {
            IcLicense = "0851",
            InvestorClass = null,
            CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder>()
        };

        // Act
        var actual = fundInfo.VerifyCustomerAccess(customer);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(FundProjectType.Unknown, true)]
    [InlineData(FundProjectType.GeneralInvestorsFund, true)]
    [InlineData(FundProjectType.InstitutionalInvestorsFund, true)]
    [InlineData(FundProjectType.CorporateInvestorsFund, true)]
    [InlineData(FundProjectType.ThaiGovernmentFund, true)]
    [InlineData(FundProjectType.PensionReserveFund, true)]
    public void Should_Return_Expected_When_VerifyCustomerAccess_And_FundFilters_Is_Empty(FundProjectType projectType, bool expected)
    {
        // Arrange
        var fundInfo = NewFundInfo(projectType);
        var customer = new CustomerAccountDetail
        {
            IcLicense = "0851",
            InvestorClass = InvestorClass.Institutional,
            CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder>()
        };

        // Act
        var actual = fundInfo.VerifyCustomerAccess(customer);

        // Assert
        Assert.Equal(expected, actual);
    }

    private FundInfo NewFundInfo(FundProjectType projectType)
    {
        return new FundInfo("กองทุนเปิด ทิสโก้ Next Generation Internet ชนิดผู้ลงทุนทั่วไป", "APP", "some url", "014")
        {
            Nav = 10,
            InstrumentCategory = "Funds",
            FirstMinBuyAmount = 1000,
            NextMinBuyAmount = 1000,
            MinSellAmount = 1000,
            MinSellUnit = 1000,
            MinBalanceAmount = 100,
            MinBalanceUnit = 10,
            PiBuyCutOffTime = DateTime.UtcNow.AddHours(2),
            PiSellCutOffTime = DateTime.UtcNow.AddHours(2),
            ProjectType = projectType
        };
    }
}
