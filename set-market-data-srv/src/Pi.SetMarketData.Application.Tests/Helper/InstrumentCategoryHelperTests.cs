using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Helper;

namespace Pi.SetMarketData.Application.Tests.Helper
{
    public class InstrumentCategoryHelperTests
    {
        public InstrumentCategoryHelperTests()
        { }

        [Theory]
        [InlineData(InstrumentConstants.CS, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiStocks)] // Common Stock
        [InlineData(InstrumentConstants.PS, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiStocks)] // Preferred Stock
        [InlineData(InstrumentConstants.ETF, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiETFs)] // Exchange Traded Fund
        [InlineData(InstrumentConstants.W, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiStockWarrants)] // Warrant
        [InlineData(InstrumentConstants.DWC, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiDerivativeWarrants)] // Derivative Call Warrants
        [InlineData(InstrumentConstants.DWP, InstrumentConstants.SET, 11, "", InstrumentConstants.ThaiDerivativeWarrants)] // Derivative Put Warrants
        [InlineData(InstrumentConstants.DWC, InstrumentConstants.SET, 13, "", InstrumentConstants.ForeignDerivativeWarrants)] // Foreign Derivative Call Warrants
        [InlineData(InstrumentConstants.DWP, InstrumentConstants.SET, 13, "", InstrumentConstants.ForeignDerivativeWarrants)] // Foreign Derivative Put Warrants
        [InlineData(InstrumentConstants.DR, InstrumentConstants.SET, 13, "", InstrumentConstants.DRs)] // Depositary Receipts
        [InlineData(InstrumentConstants.TSR, InstrumentConstants.SET, 99, "", InstrumentConstants.Ignored)] // Transferable Subscription Rights
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXE, 99, "", InstrumentConstants.Ignored)] // Cash-Settled Future
        [InlineData(InstrumentConstants.CMB, InstrumentConstants.MAI, 99, "", InstrumentConstants.Ignored)] // Combination
        [InlineData(InstrumentConstants.UL, InstrumentConstants.SET, 99, "", InstrumentConstants.Ignored)] // Underlying
        [InlineData(InstrumentConstants.SPT, InstrumentConstants.TXE, 99, "", InstrumentConstants.Ignored)] // Spot
        [InlineData(InstrumentConstants.WEC, InstrumentConstants.SET, 99, "", InstrumentConstants.Ignored)] // Weekly European Call Option
        [InlineData(InstrumentConstants.WEP, InstrumentConstants.TXE, 99, "", InstrumentConstants.Ignored)] // Weekly European Put Option
        [InlineData(InstrumentConstants.CSF, InstrumentConstants.SET, 999, "", InstrumentConstants.Unstructured)] // Foreign Stock
        [InlineData(InstrumentConstants.PS, InstrumentConstants.TXR, 999, "", InstrumentConstants.Unstructured)] // Preferred Foreign Stock
        [InlineData(InstrumentConstants.UT, InstrumentConstants.TXA, 999, "", InstrumentConstants.Unstructured)] // Unit Trust
        [InlineData(InstrumentConstants.CMB, InstrumentConstants.TXD, 999, "", InstrumentConstants.Unstructured)] // Combination
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXA, 999, "", InstrumentConstants.Unstructured)] // Cash-Settled Future
        [InlineData(InstrumentConstants.FP, InstrumentConstants.TXR, 999, "", InstrumentConstants.Unstructured)] // Physical Delivery Futures
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXI, 101, InstrumentConstants.SET50, InstrumentConstants.SET50IndexFutures)] // SET50 Index Futures
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXI, 101, "", InstrumentConstants.SectorIndexFutures)] // Sector Index Futures
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXS, 102, "", InstrumentConstants.SingleStockFutures)] // Single Stock Futures
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXM, 104, "", InstrumentConstants.PreciousMetalFutures)] // Precious Metal Futures
        [InlineData(InstrumentConstants.FC, InstrumentConstants.TXC, 105, "", InstrumentConstants.CurrencyFutures)] // Currency Futures
        [InlineData(InstrumentConstants.OEC, InstrumentConstants.TXI, 101, "", InstrumentConstants.SET50IndexOptions)] // SET50 Index Options (European Call)
        [InlineData(InstrumentConstants.OEP, InstrumentConstants.TXI, 101, "", InstrumentConstants.SET50IndexOptions)] // SET50 Index Options (European Put)
        [InlineData(InstrumentConstants.IDX, "", 0, "", InstrumentConstants.SETIndices)] // SET Indices
        public void InstrumentCategoryHelper_Returns_Correct_Category(string financialProduct, string marketSegment, int marketCode, string underlyingName, string expectedCategory)
        {
            // Act
            string result = InstrumentCategoryHelper.MapInstrumentCategory(financialProduct, marketSegment, marketCode, underlyingName);

            // Assert
            Assert.Equal(expectedCategory, result);
        }

        [Theory]
        [InlineData(InstrumentConstants.SET, InstrumentConstants.SET)] // Valid SET market segment
        [InlineData(InstrumentConstants.TXE, InstrumentConstants.TXE)] // Valid TXE market segment
        [InlineData(InstrumentConstants.MAI, InstrumentConstants.MAI)] // Valid MAI market segment
        [InlineData(InstrumentConstants.TXI, InstrumentConstants.TXI)] // Valid TXI market segment
        [InlineData(InstrumentConstants.TXS, InstrumentConstants.TXS)] // Valid TXS market segment
        [InlineData(InstrumentConstants.TXM, InstrumentConstants.TXM)] // Valid TXM market segment
        [InlineData(InstrumentConstants.TXC, InstrumentConstants.TXC)] // Valid TXC market segment
        [InlineData("Invalid", "")] // Invalid market segment
        public void MapMarketSegment_Returns_Correct_Value(string marketSegmentValue, string expectedSegment)
        {
            // Act
            string result = InstrumentCategoryHelper.MapMarketSegment(marketSegmentValue);

            // Assert
            Assert.Equal(expectedSegment, result);
        }

        [Theory]
        [InlineData(11, 11)] // Valid market code 11
        [InlineData(13, 13)] // Valid market code 13
        [InlineData(101, 101)] // Valid market code 101
        [InlineData(102, 102)] // Valid market code 102
        [InlineData(104, 104)] // Valid market code 104
        [InlineData(105, 105)] // Valid market code 105
        [InlineData(999, 0)] // Invalid market code
        [InlineData(0, 0)] // Zero market code (not mapped)
        public void MapMarketCode_Returns_Correct_Value(int marketCodeValue, int expectedCode)
        {
            // Act
            int result = InstrumentCategoryHelper.MapMarketCode(marketCodeValue);

            // Assert
            Assert.Equal(expectedCode, result);
        }
    }
}
